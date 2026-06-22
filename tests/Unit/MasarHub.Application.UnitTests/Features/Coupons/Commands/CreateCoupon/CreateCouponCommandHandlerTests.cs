using FluentAssertions;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Features.Coupons.Commands.CreateCoupon;
using MasarHub.Domain.Modules.Payments;
using Moq;

namespace MasarHub.Application.UnitTests.Features.Coupons.Commands.CreateCoupon
{
    [Trait("UnitTests.Feature.Coupons", "CreateCoupon")]
    public sealed class CreateCouponCommandHandlerTests
    {
        private readonly Mock<IRepository<Coupon>> _couponRepositoryMock;
        private readonly Mock<ICouponQuery> _couponQueryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly CreateCouponCommandHandler _sut;
        private static readonly Guid UserId = Guid.NewGuid();

        public CreateCouponCommandHandlerTests()
        {
            _couponRepositoryMock = new Mock<IRepository<Coupon>>();
            _couponQueryMock = new Mock<ICouponQuery>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _sut = new CreateCouponCommandHandler(
                _couponRepositoryMock.Object,
                _couponQueryMock.Object,
                _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Handle_CourseNotFound_ReturnsNotFoundError()
        {
            var courseId = Guid.NewGuid();
            var command = new CreateCouponCommand(courseId, UserId, "DISCOUNT10", 10m, DiscountType.Fixed, DateTimeOffset.UtcNow.AddDays(30), 100);

            _couponQueryMock
                .Setup(x => x.GetCreateCouponDataAsync(command.Code, courseId, UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CreateCouponData(false, false, false));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "course.not_found");
            _couponRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Coupon>(), It.IsAny<CancellationToken>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_NotCourseOwner_ReturnsForbiddenError()
        {
            var courseId = Guid.NewGuid();
            var command = new CreateCouponCommand(courseId, UserId, "DISCOUNT10", 10m, DiscountType.Fixed, DateTimeOffset.UtcNow.AddDays(30), 100);

            _couponQueryMock
                .Setup(x => x.GetCreateCouponDataAsync(command.Code, courseId, UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CreateCouponData(false, true, false));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "course.access_denied");
            _couponRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Coupon>(), It.IsAny<CancellationToken>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_CodeAlreadyExists_ReturnsConflictError()
        {
            var courseId = Guid.NewGuid();
            var command = new CreateCouponCommand(courseId, UserId, "DISCOUNT10", 10m, DiscountType.Fixed, DateTimeOffset.UtcNow.AddDays(30), 100);

            _couponQueryMock
                .Setup(x => x.GetCreateCouponDataAsync(command.Code, courseId, UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CreateCouponData(true, true, true));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "coupon.code_already_exists");
            _couponRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Coupon>(), It.IsAny<CancellationToken>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_InvalidExpirationDate_ReturnsDomainError()
        {
            var courseId = Guid.NewGuid();
            var command = new CreateCouponCommand(courseId, UserId, "DISCOUNT10", 10m, DiscountType.Fixed, DateTimeOffset.UtcNow.AddDays(-1), 100);

            _couponQueryMock
                .Setup(x => x.GetCreateCouponDataAsync(command.Code, courseId, UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CreateCouponData(false, true, true));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "Coupon.InvalidExpiration");
            _couponRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Coupon>(), It.IsAny<CancellationToken>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ValidCommand_ReturnsSuccessResponse()
        {
            var courseId = Guid.NewGuid();
            var command = new CreateCouponCommand(courseId, UserId, "DISCOUNT10", 10m, DiscountType.Fixed, DateTimeOffset.UtcNow.AddDays(30), 100);

            _couponQueryMock
                .Setup(x => x.GetCreateCouponDataAsync(command.Code, courseId, UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CreateCouponData(false, true, true));
            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Code.Should().Be("DISCOUNT10");
            result.Value.Value.Should().Be(10m);
            result.Value.Type.Should().Be(DiscountType.Fixed);
            result.Value.CourseId.Should().Be(courseId);
            _couponRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Coupon>(), It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
