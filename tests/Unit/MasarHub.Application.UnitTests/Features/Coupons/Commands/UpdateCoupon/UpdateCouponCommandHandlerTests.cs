using FluentAssertions;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Features.Coupons.Commands.UpdateCoupon;
using MasarHub.Domain.Modules.Payments;
using Moq;

namespace MasarHub.Application.UnitTests.Features.Coupons.Commands.UpdateCoupon
{
    [Trait("UnitTests.Feature.Coupons", "UpdateCoupon")]
    public sealed class UpdateCouponCommandHandlerTests
    {
        private readonly Mock<IRepository<Coupon>> _couponRepositoryMock;
        private readonly Mock<ICouponQuery> _couponQueryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly UpdateCouponCommandHandler _sut;
        private static readonly Guid UserId = Guid.NewGuid();
        private static readonly Guid CourseId = Guid.NewGuid();
        private static readonly Guid CouponId = Guid.NewGuid();

        public UpdateCouponCommandHandlerTests()
        {
            _couponRepositoryMock = new Mock<IRepository<Coupon>>();
            _couponQueryMock = new Mock<ICouponQuery>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _sut = new UpdateCouponCommandHandler(
                _couponRepositoryMock.Object,
                _couponQueryMock.Object,
                _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Handle_CouponNotFound_ReturnsNotFoundError()
        {
            var command = new UpdateCouponCommand(CourseId, CouponId, UserId, 20m, null, null);

            _couponQueryMock
                .Setup(x => x.GetCouponDataAsync(CouponId, UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((CouponData?)null);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "coupon.not_found");
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_NotCourseOwner_ReturnsForbiddenError()
        {
            var command = new UpdateCouponCommand(CourseId, CouponId, UserId, 20m, null, null);

            _couponQueryMock
                .Setup(x => x.GetCouponDataAsync(CouponId, UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CouponData(CourseId, false));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "course.access_denied");
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_CourseMismatch_ReturnsConflictError()
        {
            var command = new UpdateCouponCommand(Guid.NewGuid(), CouponId, UserId, 20m, null, null);

            _couponQueryMock
                .Setup(x => x.GetCouponDataAsync(CouponId, UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CouponData(CourseId, true));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "coupon.course_mismatch");
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_UpdateValue_Success()
        {
            var coupon = Coupon.Create("DISCOUNT", 10m, DiscountType.Fixed, DateTimeOffset.UtcNow.AddDays(30), 100, CourseId).Value;
            var command = new UpdateCouponCommand(CourseId, CouponId, UserId, 20m, null, null);

            _couponQueryMock
                .Setup(x => x.GetCouponDataAsync(CouponId, UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CouponData(CourseId, true));
            _couponRepositoryMock
                .Setup(x => x.GetByIdAsync(CouponId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(coupon);
            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            coupon.Value.Should().Be(20m);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_UpdateExpirationDate_Success()
        {
            var futureDate = DateTimeOffset.UtcNow.AddDays(60);
            var coupon = Coupon.Create("DISCOUNT", 10m, DiscountType.Fixed, DateTimeOffset.UtcNow.AddDays(30), 100, CourseId).Value;
            var command = new UpdateCouponCommand(CourseId, CouponId, UserId, null, futureDate, null);

            _couponQueryMock
                .Setup(x => x.GetCouponDataAsync(CouponId, UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CouponData(CourseId, true));
            _couponRepositoryMock
                .Setup(x => x.GetByIdAsync(CouponId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(coupon);
            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            coupon.ExpirationDate.Should().Be(futureDate);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_UpdateUsageLimit_Success()
        {
            var coupon = Coupon.Create("DISCOUNT", 10m, DiscountType.Fixed, DateTimeOffset.UtcNow.AddDays(30), 100, CourseId).Value;
            var command = new UpdateCouponCommand(CourseId, CouponId, UserId, null, null, 50);

            _couponQueryMock
                .Setup(x => x.GetCouponDataAsync(CouponId, UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CouponData(CourseId, true));
            _couponRepositoryMock
                .Setup(x => x.GetByIdAsync(CouponId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(coupon);
            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            coupon.UsageLimit.Should().Be(50);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_UpdateInvalidExpirationDate_ReturnsDomainError()
        {
            var coupon = Coupon.Create("DISCOUNT", 10m, DiscountType.Fixed, DateTimeOffset.UtcNow.AddDays(30), 100, CourseId).Value;
            var command = new UpdateCouponCommand(CourseId, CouponId, UserId, null, DateTimeOffset.UtcNow.AddDays(-1), null);

            _couponQueryMock
                .Setup(x => x.GetCouponDataAsync(CouponId, UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CouponData(CourseId, true));
            _couponRepositoryMock
                .Setup(x => x.GetByIdAsync(CouponId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(coupon);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "Coupon.InvalidExpiration");
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
