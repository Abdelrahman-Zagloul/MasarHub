using FluentAssertions;
using MasarHub.Application.Features.Coupons.Commands.DeleteCoupon;

namespace MasarHub.Application.UnitTests.Features.Coupons.Commands.DeleteCoupon
{
    [Trait("UnitTests.Feature.Coupons", "DeleteCoupon")]
    public sealed class DeleteCouponCommandValidatorTests
    {
        private readonly DeleteCouponCommandValidator _sut = new();

        [Fact]
        public void Validate_EmptyCourseId_HasValidationError()
        {
            var command = new DeleteCouponCommand(Guid.Empty, Guid.NewGuid(), Guid.NewGuid());

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "CourseId");
        }

        [Fact]
        public void Validate_EmptyCouponId_HasValidationError()
        {
            var command = new DeleteCouponCommand(Guid.NewGuid(), Guid.Empty, Guid.NewGuid());

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "CouponId");
        }

        [Fact]
        public void Validate_ValidCommand_NoValidationErrors()
        {
            var command = new DeleteCouponCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

            var result = _sut.Validate(command);

            result.IsValid.Should().BeTrue();
        }
    }
}
