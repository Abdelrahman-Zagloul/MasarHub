using FluentAssertions;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Features.Coupons.Queries.GetCouponById;
using MasarHub.Application.Features.Coupons.Queries.GetCoupons;
using MasarHub.Domain.Modules.Payments;
using Moq;

namespace MasarHub.Application.UnitTests.Features.Coupons.Queries.GetCouponById
{
    [Trait("UnitTests.Feature.Coupons", "GetCouponById")]
    public sealed class GetCouponByIdQueryHandlerTests
    {
        private readonly Mock<ICouponQuery> _couponQueryMock;
        private readonly GetCouponByIdQueryHandler _sut;

        public GetCouponByIdQueryHandlerTests()
        {
            _couponQueryMock = new Mock<ICouponQuery>();
            _sut = new GetCouponByIdQueryHandler(_couponQueryMock.Object);
        }

        [Fact]
        public async Task Handle_ReturnsCoupon()
        {
            var courseId = Guid.NewGuid();
            var couponId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var coupon = new CouponResponse
            {
                Id = couponId,
                Code = "SUMMER",
                Value = 20m,
                Type = DiscountType.Fixed,
                CourseId = courseId,
                ExpirationDate = DateTimeOffset.UtcNow.AddDays(30),
                UsageLimit = 100,
                UsedCount = 0,
                CreatedAt = DateTimeOffset.UtcNow
            };

            _couponQueryMock
                .Setup(x => x.GetByIdAsync(couponId, courseId, userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CouponDetails(coupon, true));

            var result = await _sut.Handle(new GetCouponByIdQuery(courseId, couponId, userId), CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Code.Should().Be("SUMMER");
        }

        [Fact]
        public async Task Handle_CouponNotFound_ReturnsNotFound()
        {
            var courseId = Guid.NewGuid();
            var couponId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            _couponQueryMock
                .Setup(x => x.GetByIdAsync(couponId, courseId, userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CouponDetails(null, false));

            var result = await _sut.Handle(new GetCouponByIdQuery(courseId, couponId, userId), CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors[0].Code.Should().Be("coupon.not_found");
        }

        [Fact]
        public async Task Handle_NotOwner_ReturnsForbidden()
        {
            var courseId = Guid.NewGuid();
            var couponId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var coupon = new CouponResponse
            {
                Id = couponId,
                Code = "SUMMER",
                Value = 20m,
                Type = DiscountType.Fixed,
                CourseId = courseId,
                ExpirationDate = DateTimeOffset.UtcNow.AddDays(30),
                UsageLimit = 100,
                UsedCount = 0,
                CreatedAt = DateTimeOffset.UtcNow
            };

            _couponQueryMock
                .Setup(x => x.GetByIdAsync(couponId, courseId, userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CouponDetails(coupon, false));

            var result = await _sut.Handle(new GetCouponByIdQuery(courseId, couponId, userId), CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors[0].Code.Should().Be("course.access_denied");
        }
    }
}
