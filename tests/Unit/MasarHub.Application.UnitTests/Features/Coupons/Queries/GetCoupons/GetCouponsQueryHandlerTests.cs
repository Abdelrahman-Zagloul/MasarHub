using FluentAssertions;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Features.Coupons.Queries.GetCoupons;
using MasarHub.Domain.Modules.Payments;
using Moq;

namespace MasarHub.Application.UnitTests.Features.Coupons.Queries.GetCoupons
{
    [Trait("UnitTests.Feature.Coupons", "GetCoupons")]
    public sealed class GetCouponsQueryHandlerTests
    {
        private readonly Mock<ICouponQuery> _couponQueryMock;
        private readonly GetCouponsQueryHandler _sut;

        public GetCouponsQueryHandlerTests()
        {
            _couponQueryMock = new Mock<ICouponQuery>();
            _sut = new GetCouponsQueryHandler(_couponQueryMock.Object);
        }

        [Fact]
        public async Task Handle_ReturnsCoupons()
        {
            var courseId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var query = new GetCouponsQuery(courseId, userId);
            var coupons = new List<CouponResponse>
            {
                new()
                {
                    Id = Guid.NewGuid(), Code = "SUMMER", Value = 20m, Type = DiscountType.Fixed,
                    CourseId = courseId, ExpirationDate = DateTimeOffset.UtcNow.AddDays(30),
                    UsageLimit = 100, UsedCount = 0, CreatedAt = DateTimeOffset.UtcNow
                },
                new()
                {
                    Id = Guid.NewGuid(), Code = "WINTER", Value = 15m, Type = DiscountType.Percentage,
                    CourseId = courseId, ExpirationDate = DateTimeOffset.UtcNow.AddDays(60),
                    UsageLimit = 50, UsedCount = 5, CreatedAt = DateTimeOffset.UtcNow
                }
            };

            _couponQueryMock
                .Setup(x => x.GetAllAsync(query, userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CouponListResult(coupons, true, true));

            var result = await _sut.Handle(query, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().HaveCount(2);
            result.Value[0].Code.Should().Be("SUMMER");
            result.Value[1].Code.Should().Be("WINTER");
        }

        [Fact]
        public async Task Handle_EmptyResult_ReturnsEmptyList()
        {
            var userId = Guid.NewGuid();
            var courseId = Guid.NewGuid();
            var query = new GetCouponsQuery(courseId, userId);

            _couponQueryMock
                .Setup(x => x.GetAllAsync(query, userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CouponListResult([], true, true));

            var result = await _sut.Handle(query, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEmpty();
        }

        [Fact]
        public async Task Handle_CourseNotFound_ReturnsNotFound()
        {
            var courseId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var query = new GetCouponsQuery(courseId, userId);

            _couponQueryMock
                .Setup(x => x.GetAllAsync(query, userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CouponListResult([], false, false));

            var result = await _sut.Handle(query, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors[0].Code.Should().Be("course.not_found");
        }

        [Fact]
        public async Task Handle_NotOwner_ReturnsForbidden()
        {
            var courseId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var query = new GetCouponsQuery(courseId, userId);

            _couponQueryMock
                .Setup(x => x.GetAllAsync(query, userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CouponListResult([], true, false));

            var result = await _sut.Handle(query, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors[0].Code.Should().Be("course.access_denied");
        }
    }
}
