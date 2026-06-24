using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Abstractions.Services;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Application.Features.Carts.Models;
using MasarHub.Application.Features.Orders.Shared;
using MasarHub.Domain.Modules.Orders;
using MasarHub.Domain.Modules.Payments;
using MediatR;

namespace MasarHub.Application.Features.Orders.Commands.CreateOrder
{
    public sealed class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Result<OrderResponse>>
    {
        private readonly ICartService _cartService;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<Coupon> _couponRepository;
        private readonly IRepository<CouponUsage> _couponUsageRepository;
        private readonly IOrderQuery _orderQuery;
        private readonly IUnitOfWork _unitOfWork;

        public CreateOrderCommandHandler(ICartService cartService, IRepository<Order> orderRepository, IRepository<Coupon> couponRepository, IRepository<CouponUsage> couponUsageRepository, IOrderQuery orderQuery, IUnitOfWork unitOfWork)
        {
            _cartService = cartService;
            _orderRepository = orderRepository;
            _couponRepository = couponRepository;
            _couponUsageRepository = couponUsageRepository;
            _orderQuery = orderQuery;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<OrderResponse>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var cart = await _cartService.GetOrCreateAsync(request.UserId, cancellationToken);
            if (cart.Items.Count == 0)
                return Error.Conflict("cart.empty");

            var cartCourseIds = cart.Items.Select(i => i.CourseId).ToList();
            var orderData = await _orderQuery.GetCreateOrderDataAsync(request.UserId, cartCourseIds, request.Coupons, cancellationToken);
            if (orderData.Courses.Count != cartCourseIds.Count)
                return Error.NotFound("course.not_found");


            var appliedCouponsResult = ValidateCoupons(request.Coupons, cartCourseIds.ToHashSet(), orderData);
            if (appliedCouponsResult.IsFailure)
                return appliedCouponsResult.Errors[0];

            var trackedCoupons = appliedCouponsResult.Value;
            if (trackedCoupons.Count > 0)
                _couponRepository.AttachRange(trackedCoupons.Values);

            var orderResult = Order.Create(request.UserId, GenerateOrderNumber());
            if (orderResult.IsFailure)
                return orderResult.Error;

            var order = orderResult.Value;


            var buildResult = await BuildOrderItemsAsync(order, cart.Items, orderData.Courses.ToDictionary(c => c.Id), trackedCoupons, cancellationToken);
            if (buildResult.IsFailure)
                return buildResult.Errors[0];

            await _orderRepository.AddAsync(order, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _cartService.ClearAsync(request.UserId, cancellationToken);

            return new OrderResponse
            (
                order.Id,
                order.OrderNumber,
                order.FinalAmount,
                order.Status,
                order.CreatedAt,
                order.Items.Select(item => new OrderItemResponse(
                    item.CourseId,
                    item.CourseTitle,
                    item.OriginalPrice,
                    item.DiscountAmount,
                    item.FinalPrice,
                    item.CouponId
                )).ToList()
            );
        }

        private static Result<Dictionary<Guid, Coupon>> ValidateCoupons(List<CourseCoupon>? coupons, HashSet<Guid> cartCourseIds, CreateOrderData orderData)
        {
            if (coupons == null || coupons.Count == 0)
                return new Dictionary<Guid, Coupon>();

            var applied = new Dictionary<Guid, Coupon>();
            var existingUsageIds = orderData.ExistingUsageIds.ToHashSet();

            foreach (var courseCoupon in coupons)
            {
                var coupon = orderData.Coupons.FirstOrDefault(c => c.Code == courseCoupon.CouponCode && c.CourseId == courseCoupon.CourseId);
                if (coupon == null)
                    return Error.NotFound("coupon.not_found");

                if (!cartCourseIds.Contains(courseCoupon.CourseId))
                    return Error.Conflict("coupon.not_applicable_to_cart");

                if (!applied.TryAdd(courseCoupon.CourseId, coupon))
                    return Error.Conflict("coupon.duplicate_for_course");

                if (existingUsageIds.Contains(coupon.Id))
                    return Error.Conflict("coupon.already_used");
            }

            return applied;
        }

        private async Task<Result> BuildOrderItemsAsync(
            Order order,
            List<CartItem> cartItems,
            Dictionary<Guid, CourseCartData> coursesDataById,
            Dictionary<Guid, Coupon> appliedCoupons,
            CancellationToken cancellationToken)
        {
            foreach (var cartItem in cartItems)
            {
                if (!coursesDataById.TryGetValue(cartItem.CourseId, out var courseData))
                    return Error.NotFound("course.not_found");

                if (!courseData.IsPublished)
                    return Error.BadRequest("course.not_published");

                decimal discountAmount = 0;
                Guid? couponId = null;
                if (appliedCoupons.TryGetValue(cartItem.CourseId, out var coupon))
                {
                    var applyResult = coupon.ApplyCoupon(cartItem.CourseId);
                    if (applyResult.IsFailure)
                        return applyResult.Error;

                    var discountAmountResult = coupon.CalculateDiscount(courseData.Price, coupon.CourseId);
                    if (discountAmountResult.IsFailure)
                        return discountAmountResult.Error;

                    discountAmount = discountAmountResult.Value;
                    couponId = coupon.Id;
                    _couponRepository.Update(coupon);

                    var couponUsage = CouponUsage.Create(coupon.Id, order.UserId).Value;
                    await _couponUsageRepository.AddAsync(couponUsage, cancellationToken);
                }

                var itemResult = OrderItem.Create(courseData.Id, courseData.Title, courseData.Price, discountAmount, couponId);
                if (itemResult.IsFailure)
                    return itemResult.Error;

                var addResult = order.AddItem(itemResult.Value);
                if (addResult.IsFailure)
                    return addResult.Error;
            }
            return Result.Success();
        }

        private static string GenerateOrderNumber()
        {
            return $"ORD-{DateTimeOffset.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpperInvariant()}";
        }
    }
}