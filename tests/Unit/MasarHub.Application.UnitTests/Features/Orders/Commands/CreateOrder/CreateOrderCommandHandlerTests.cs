using FluentAssertions;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Abstractions.Services;
using MasarHub.Application.Features.Carts.Models;
using MasarHub.Application.Features.Orders.Commands.CreateOrder;
using MasarHub.Domain.Modules.Orders;
using MasarHub.Domain.Modules.Payments;
using Moq;

namespace MasarHub.Application.UnitTests.Features.Orders.Commands.CreateOrder
{
    [Trait("UnitTests.Feature.Orders", "CreateOrder")]
    public sealed class CreateOrderCommandHandlerTests
    {
        private readonly Mock<ICartService> _cartServiceMock;
        private readonly Mock<IRepository<Order>> _orderRepositoryMock;
        private readonly Mock<IRepository<Coupon>> _couponRepositoryMock;
        private readonly Mock<IRepository<CouponUsage>> _couponUsageRepositoryMock;
        private readonly Mock<IOrderQuery> _orderQueryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly CreateOrderCommandHandler _sut;
        private static readonly Guid UserId = Guid.NewGuid();

        public CreateOrderCommandHandlerTests()
        {
            _cartServiceMock = new Mock<ICartService>();
            _orderRepositoryMock = new Mock<IRepository<Order>>();
            _couponRepositoryMock = new Mock<IRepository<Coupon>>();
            _couponUsageRepositoryMock = new Mock<IRepository<CouponUsage>>();
            _orderQueryMock = new Mock<IOrderQuery>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _sut = new CreateOrderCommandHandler(
                _cartServiceMock.Object,
                _orderRepositoryMock.Object,
                _couponRepositoryMock.Object,
                _couponUsageRepositoryMock.Object,
                _orderQueryMock.Object,
                _unitOfWorkMock.Object);
        }

        private static Cart CreateCartWithTwoItems()
        {
            var cart = new Cart(UserId);
            cart.AddItem(CartItem.Create(Guid.NewGuid(), "Course 1", 100m, null));
            cart.AddItem(CartItem.Create(Guid.NewGuid(), "Course 2", 200m, null));
            return cart;
        }

        private static CreateOrderData CreateOrderData(Cart cart, List<Coupon>? coupons = null)
        {
            var courses = cart.Items.Select(i => new CourseCartData(i.CourseId, i.Title, i.Price, null, true)).ToList();
            return new CreateOrderData(courses, coupons ?? [], []);
        }

        [Fact]
        public async Task Handle_EmptyCart_ReturnsConflict()
        {
            var cart = new Cart(UserId);
            _cartServiceMock
                .Setup(x => x.GetOrCreateAsync(UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cart);

            var result = await _sut.Handle(new CreateOrderCommand(UserId, null), CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "cart.empty");
        }

        [Fact]
        public async Task Handle_CourseNotFoundInDb_ReturnsNotFound()
        {
            var cart = CreateCartWithTwoItems();

            _cartServiceMock
                .Setup(x => x.GetOrCreateAsync(UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cart);

            _orderQueryMock
                .Setup(x => x.GetCreateOrderDataAsync(UserId, It.IsAny<List<Guid>>(), It.IsAny<List<CourseCoupon>?>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CreateOrderData(
                    [new CourseCartData(cart.Items[1].CourseId, cart.Items[1].Title, cart.Items[1].Price, null, true)],
                    [],
                    []));

            var result = await _sut.Handle(new CreateOrderCommand(UserId, null), CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "course.not_found");
        }

        [Fact]
        public async Task Handle_CouponNotFound_ReturnsNotFound()
        {
            var cart = CreateCartWithTwoItems();

            _cartServiceMock
                .Setup(x => x.GetOrCreateAsync(UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cart);

            _orderQueryMock
                .Setup(x => x.GetCreateOrderDataAsync(UserId, It.IsAny<List<Guid>>(), It.IsAny<List<CourseCoupon>?>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateOrderData(cart));

            var coupons = new List<CourseCoupon> { new("INVALID", cart.Items[0].CourseId) };
            var result = await _sut.Handle(new CreateOrderCommand(UserId, coupons), CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "coupon.not_found");
        }

        [Fact]
        public async Task Handle_CouponCourseNotInCart_ReturnsConflict()
        {
            var cart = CreateCartWithTwoItems();
            var otherCourseId = Guid.NewGuid();
            var coupon = Coupon.Create("OTHER", 10m, DiscountType.Fixed, DateTimeOffset.UtcNow.AddDays(30), 100, otherCourseId).Value;

            _cartServiceMock
                .Setup(x => x.GetOrCreateAsync(UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cart);

            _orderQueryMock
                .Setup(x => x.GetCreateOrderDataAsync(UserId, It.IsAny<List<Guid>>(), It.IsAny<List<CourseCoupon>?>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CreateOrderData(
                    cart.Items.Select(i => new CourseCartData(i.CourseId, i.Title, i.Price, null, true)).ToList(),
                    [coupon],
                    []));

            var coupons = new List<CourseCoupon> { new("OTHER", otherCourseId) };
            var result = await _sut.Handle(new CreateOrderCommand(UserId, coupons), CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "coupon.not_applicable_to_cart");
        }

        [Fact]
        public async Task Handle_DuplicateCouponForCourse_ReturnsConflict()
        {
            var cart = CreateCartWithTwoItems();
            var courseId = cart.Items[0].CourseId;
            var coupon = Coupon.Create("SUMMER", 10m, DiscountType.Fixed, DateTimeOffset.UtcNow.AddDays(30), 100, courseId).Value;

            _cartServiceMock
                .Setup(x => x.GetOrCreateAsync(UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cart);

            _orderQueryMock
                .Setup(x => x.GetCreateOrderDataAsync(UserId, It.IsAny<List<Guid>>(), It.IsAny<List<CourseCoupon>?>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CreateOrderData(
                    cart.Items.Select(i => new CourseCartData(i.CourseId, i.Title, i.Price, null, true)).ToList(),
                    [coupon],
                    []));

            var coupons = new List<CourseCoupon>
            {
                new("SUMMER", courseId),
                new("SUMMER", courseId)
            };
            var result = await _sut.Handle(new CreateOrderCommand(UserId, coupons), CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "coupon.duplicate_for_course");
        }

        [Fact]
        public async Task Handle_CouponAlreadyUsed_ReturnsConflict()
        {
            var cart = CreateCartWithTwoItems();
            var courseId = cart.Items[0].CourseId;
            var coupon = Coupon.Create("SUMMER", 10m, DiscountType.Fixed, DateTimeOffset.UtcNow.AddDays(30), 100, courseId).Value;

            _cartServiceMock
                .Setup(x => x.GetOrCreateAsync(UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cart);

            _orderQueryMock
                .Setup(x => x.GetCreateOrderDataAsync(UserId, It.IsAny<List<Guid>>(), It.IsAny<List<CourseCoupon>?>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CreateOrderData(
                    cart.Items.Select(i => new CourseCartData(i.CourseId, i.Title, i.Price, null, true)).ToList(),
                    [coupon],
                    [coupon.Id]));

            var coupons = new List<CourseCoupon> { new("SUMMER", courseId) };
            var result = await _sut.Handle(new CreateOrderCommand(UserId, coupons), CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "coupon.already_used");
        }

        [Fact]
        public async Task Handle_ValidOrderWithoutCoupon_ReturnsSuccess()
        {
            var cart = CreateCartWithTwoItems();

            _cartServiceMock
                .Setup(x => x.GetOrCreateAsync(UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cart);

            _orderQueryMock
                .Setup(x => x.GetCreateOrderDataAsync(UserId, It.IsAny<List<Guid>>(), It.IsAny<List<CourseCoupon>?>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateOrderData(cart));

            var result = await _sut.Handle(new CreateOrderCommand(UserId, null), CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.FinalAmount.Should().Be(300m);
            result.Value.Status.Should().Be(OrderStatus.PendingPayment);
            _orderRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _cartServiceMock.Verify(x => x.ClearAsync(UserId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ValidOrderWithCoupon_ReturnsSuccess()
        {
            var cart = CreateCartWithTwoItems();
            var courseId = cart.Items[0].CourseId;
            var coupon = Coupon.Create("SUMMER", 10m, DiscountType.Fixed, DateTimeOffset.UtcNow.AddDays(30), 100, courseId).Value;

            _cartServiceMock
                .Setup(x => x.GetOrCreateAsync(UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cart);

            _orderQueryMock
                .Setup(x => x.GetCreateOrderDataAsync(UserId, It.IsAny<List<Guid>>(), It.IsAny<List<CourseCoupon>?>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CreateOrderData(
                    cart.Items.Select(i => new CourseCartData(i.CourseId, i.Title, i.Price, null, true)).ToList(),
                    [coupon],
                    []));

            var coupons = new List<CourseCoupon> { new("SUMMER", courseId) };
            var result = await _sut.Handle(new CreateOrderCommand(UserId, coupons), CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.FinalAmount.Should().Be(290m);
            _couponRepositoryMock.Verify(x => x.Update(It.IsAny<Coupon>()), Times.Once);
            _couponUsageRepositoryMock.Verify(x => x.AddAsync(It.IsAny<CouponUsage>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ValidOrderWithMultipleCoupons_ReturnsSuccess()
        {
            var cart = CreateCartWithTwoItems();
            var courseId1 = cart.Items[0].CourseId;
            var courseId2 = cart.Items[1].CourseId;
            var coupon1 = Coupon.Create("SUMMER", 10m, DiscountType.Fixed, DateTimeOffset.UtcNow.AddDays(30), 100, courseId1).Value;
            var coupon2 = Coupon.Create("WINTER", 20m, DiscountType.Fixed, DateTimeOffset.UtcNow.AddDays(30), 100, courseId2).Value;

            _cartServiceMock
                .Setup(x => x.GetOrCreateAsync(UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cart);

            _orderQueryMock
                .Setup(x => x.GetCreateOrderDataAsync(UserId, It.IsAny<List<Guid>>(), It.IsAny<List<CourseCoupon>?>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CreateOrderData(
                    cart.Items.Select(i => new CourseCartData(i.CourseId, i.Title, i.Price, null, true)).ToList(),
                    [coupon1, coupon2],
                    []));

            var coupons = new List<CourseCoupon>
            {
                new("SUMMER", courseId1),
                new("WINTER", courseId2)
            };
            var result = await _sut.Handle(new CreateOrderCommand(UserId, coupons), CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.FinalAmount.Should().Be(270m);
            _couponRepositoryMock.Verify(x => x.Update(It.IsAny<Coupon>()), Times.Exactly(2));
            _couponUsageRepositoryMock.Verify(x => x.AddAsync(It.IsAny<CouponUsage>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        }

        [Fact]
        public async Task Handle_ExpiredCoupon_ReturnsFailure()
        {
            var cart = CreateCartWithTwoItems();
            var courseId = cart.Items[0].CourseId;
            var coupon = Coupon.Create("EXPIRED", 10m, DiscountType.Fixed, DateTimeOffset.UtcNow.AddDays(1), 100, courseId).Value;
            typeof(Coupon).GetProperty(nameof(Coupon.ExpirationDate))!
                .SetValue(coupon, DateTimeOffset.UtcNow.AddDays(-1));

            _cartServiceMock
                .Setup(x => x.GetOrCreateAsync(UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cart);

            _orderQueryMock
                .Setup(x => x.GetCreateOrderDataAsync(UserId, It.IsAny<List<Guid>>(), It.IsAny<List<CourseCoupon>?>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CreateOrderData(
                    cart.Items.Select(i => new CourseCartData(i.CourseId, i.Title, i.Price, null, true)).ToList(),
                    [coupon],
                    []));

            var coupons = new List<CourseCoupon> { new("EXPIRED", courseId) };
            var result = await _sut.Handle(new CreateOrderCommand(UserId, coupons), CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "Coupon.Expired");
        }

        [Fact]
        public async Task Handle_ExhaustedCoupon_ReturnsFailure()
        {
            var cart = CreateCartWithTwoItems();
            var courseId = cart.Items[0].CourseId;
            var coupon = Coupon.Create("EXHAUSTED", 10m, DiscountType.Fixed, DateTimeOffset.UtcNow.AddDays(30), 100, courseId).Value;
            for (int i = 0; i < 100; i++)
                coupon.ApplyCoupon(courseId);

            _cartServiceMock
                .Setup(x => x.GetOrCreateAsync(UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cart);

            _orderQueryMock
                .Setup(x => x.GetCreateOrderDataAsync(UserId, It.IsAny<List<Guid>>(), It.IsAny<List<CourseCoupon>?>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CreateOrderData(
                    cart.Items.Select(i => new CourseCartData(i.CourseId, i.Title, i.Price, null, true)).ToList(),
                    [coupon],
                    []));

            var coupons = new List<CourseCoupon> { new("EXHAUSTED", courseId) };
            var result = await _sut.Handle(new CreateOrderCommand(UserId, coupons), CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "Coupon.Exhausted");
        }

        [Fact]
        public async Task Handle_PercentageCoupon_ReturnsCorrectDiscount()
        {
            var cart = CreateCartWithTwoItems();
            var courseId = cart.Items[0].CourseId;
            var coupon = Coupon.Create("PCT", 25m, DiscountType.Percentage, DateTimeOffset.UtcNow.AddDays(30), 100, courseId).Value;

            _cartServiceMock
                .Setup(x => x.GetOrCreateAsync(UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cart);

            _orderQueryMock
                .Setup(x => x.GetCreateOrderDataAsync(UserId, It.IsAny<List<Guid>>(), It.IsAny<List<CourseCoupon>?>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CreateOrderData(
                    cart.Items.Select(i => new CourseCartData(i.CourseId, i.Title, i.Price, null, true)).ToList(),
                    [coupon],
                    []));

            var coupons = new List<CourseCoupon> { new("PCT", courseId) };
            var result = await _sut.Handle(new CreateOrderCommand(UserId, coupons), CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.FinalAmount.Should().Be(275m);
        }
    }
}
