using FluentAssertions;
using MasarHub.Application.Features.Coupons.Commands.UpdateCoupon;

namespace MasarHub.Application.UnitTests.Features.Coupons.Commands.UpdateCoupon
{
    [Trait("UnitTests.Feature.Coupons", "UpdateCoupon")]
    public sealed class UpdateCouponCommandValidatorTests
    {
        private readonly UpdateCouponCommandValidator _sut = new();

        [Fact]
        public void Validate_EmptyCourseId_HasValidationError()
        {
            var command = new UpdateCouponCommand(Guid.Empty, Guid.NewGuid(), Guid.NewGuid(), 10m, null, null);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "CourseId");
        }

        [Fact]
        public void Validate_EmptyCouponId_HasValidationError()
        {
            var command = new UpdateCouponCommand(Guid.NewGuid(), Guid.Empty, Guid.NewGuid(), 10m, null, null);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "CouponId");
        }

        [Fact]
        public void Validate_NoFieldsToUpdate_HasValidationError()
        {
            var command = new UpdateCouponCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), null, null, null);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorCode == "validation.at_least_one_field_required");
        }

        [Fact]
        public void Validate_AllFieldsProvided_NoValidationErrors()
        {
            var command = new UpdateCouponCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 20m, DateTimeOffset.UtcNow.AddDays(60), 50);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_OnlyValueProvided_NoValidationErrors()
        {
            var command = new UpdateCouponCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 20m, null, null);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_OnlyExpirationDateProvided_NoValidationErrors()
        {
            var command = new UpdateCouponCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), null, DateTimeOffset.UtcNow.AddDays(60), null);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_OnlyUsageLimitProvided_NoValidationErrors()
        {
            var command = new UpdateCouponCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), null, null, 50);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeTrue();
        }
    }
}
