using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Abstractions.Services;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
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
        private readonly IUnitOfWork _unitOfWork;

        public CreateOrderCommandHandler(ICartService cartService, IRepository<Order> orderRepository, IRepository<Coupon> couponRepository, IUnitOfWork unitOfWork)
        {
            _cartService = cartService;
            _orderRepository = orderRepository;
            _couponRepository = couponRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<OrderResponse>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var cart = await _cartService.GetOrCreateAsync(request.UserId, cancellationToken);
            if (cart.Items.Count == 0)
                return Error.Conflict("cart.empty");

            var cartCourseIds = cart.Items.Select(i => i.CourseId).ToHashSet();
            var appliedCoupons = new Dictionary<Guid, Coupon>();

            if (request.Coupons != null && request.Coupons.Count > 0)
            {
                var couponCodes = request.Coupons.Select(c => c.CouponCode).ToList();
                var courseIds = request.Coupons.Select(c => c.CourseId).ToList();
                var allCoupons = await _couponRepository.GetAllAsync(c => couponCodes.Contains(c.Code) && courseIds.Contains(c.CourseId), cancellationToken);

                foreach (var courseCoupon in request.Coupons)
                {
                    var coupon = allCoupons.FirstOrDefault(c => c.Code == courseCoupon.CouponCode && c.CourseId == courseCoupon.CourseId);
                    if (coupon == null)
                        return Error.NotFound("coupon.not_found");

                    if (!cartCourseIds.Contains(courseCoupon.CourseId))
                        return Error.Conflict("coupon.not_applicable_to_cart");

                    if (!appliedCoupons.TryAdd(courseCoupon.CourseId, coupon))
                        return Error.Conflict("coupon.duplicate_for_course");
                }
            }


            var orderResult = Order.Create(request.UserId, GenerateOrderNumber());
            if (orderResult.IsFailure)
                return orderResult.Error;

            var order = orderResult.Value;
            foreach (var cartItem in cart.Items)
            {
                decimal discountAmount = 0;
                Guid? couponId = null;

                if (appliedCoupons.TryGetValue(cartItem.CourseId, out var coupon))
                {
                    var applyResult = coupon.ApplyCoupon(cartItem.CourseId);
                    if (applyResult.IsFailure)
                        return applyResult.Error;

                    var discountAmountResult = coupon.CalculateDiscount(cartItem.Price, coupon.CourseId);
                    if (discountAmountResult.IsFailure)
                        return discountAmountResult.Error;

                    discountAmount = discountAmountResult.Value;
                    couponId = coupon.Id;

                    _couponRepository.Update(coupon);
                }

                var itemResult = OrderItem.Create(cartItem.CourseId, cartItem.Title, cartItem.Price, discountAmount, couponId);
                if (itemResult.IsFailure)
                    return itemResult.Error;

                var addResult = order.AddItem(itemResult.Value);
                if (addResult.IsFailure)
                    return addResult.Error;
            }

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

        private static string GenerateOrderNumber()
        {
            return $"ORD-{DateTimeOffset.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpperInvariant()}";
        }
    }
}
