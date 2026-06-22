using FluentAssertions;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Application.Features.Coupons.Commands.DeleteCoupon;
using MasarHub.Domain.Modules.Payments;
using Moq;

namespace MasarHub.Application.UnitTests.Features.Coupons.Commands.DeleteCoupon
{
    [Trait("UnitTests.Feature.Coupons", "DeleteCoupon")]
    public sealed class DeleteCouponCommandHandlerTests
    {
        private readonly Mock<IRepository<Coupon>> _couponRepositoryMock;
        private readonly Mock<ICouponQuery> _couponQueryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly DeleteCouponCommandHandler _sut;
        private static readonly Guid UserId = Guid.NewGuid();

        public DeleteCouponCommandHandlerTests()
        {
            _couponRepositoryMock = new Mock<IRepository<Coupon>>();
            _couponQueryMock = new Mock<ICouponQuery>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _sut = new DeleteCouponCommandHandler(
                _couponRepositoryMock.Object,
                _couponQueryMock.Object,
                _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Handle_CouponNotFound_ReturnsNotFoundError()
        {
            var command = new DeleteCouponCommand(Guid.NewGuid(), Guid.NewGuid(), UserId);

            _couponQueryMock
                .Setup(x => x.GetCouponDataAsync(command.CouponId, UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((CouponData?)null);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "coupon.not_found");
            _couponRepositoryMock.Verify(x => x.Remove(It.IsAny<Coupon>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_NotCourseOwner_ReturnsForbiddenError()
        {
            var courseId = Guid.NewGuid();
            var command = new DeleteCouponCommand(courseId, Guid.NewGuid(), UserId);

            _couponQueryMock
                .Setup(x => x.GetCouponDataAsync(command.CouponId, UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CouponData(courseId, false));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "course.access_denied");
            _couponRepositoryMock.Verify(x => x.Remove(It.IsAny<Coupon>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_CourseMismatch_ReturnsConflictError()
        {
            var otherCourseId = Guid.NewGuid();
            var couponId = Guid.NewGuid();
            var command = new DeleteCouponCommand(Guid.NewGuid(), couponId, UserId);

            _couponQueryMock
                .Setup(x => x.GetCouponDataAsync(couponId, UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CouponData(otherCourseId, true));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "coupon.course_mismatch");
            _couponRepositoryMock.Verify(x => x.Remove(It.IsAny<Coupon>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ValidRequest_RemovesAndReturnsSuccess()
        {
            var courseId = Guid.NewGuid();
            var couponId = Guid.NewGuid();
            var command = new DeleteCouponCommand(courseId, couponId, UserId);
            var coupon = Coupon.Create("DISCOUNT", 10m, DiscountType.Fixed, DateTimeOffset.UtcNow.AddDays(30), 100, courseId).Value;

            _couponQueryMock
                .Setup(x => x.GetCouponDataAsync(couponId, UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CouponData(courseId, true));
            _couponRepositoryMock
                .Setup(x => x.GetByIdAsync(couponId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(coupon);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            _couponRepositoryMock.Verify(x => x.Remove(coupon), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
