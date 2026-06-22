using FluentAssertions;
using MasarHub.Application.Features.Coupons.Commands.CreateCoupon;
using MasarHub.Domain.Modules.Payments;

namespace MasarHub.Application.UnitTests.Features.Coupons.Commands.CreateCoupon
{
    [Trait("UnitTests.Feature.Coupons", "CreateCoupon")]
    public sealed class CreateCouponCommandValidatorTests
    {
        private readonly CreateCouponCommandValidator _sut = new();

        [Fact]
        public void Validate_EmptyCourseId_HasValidationError()
        {
            var command = new CreateCouponCommand(Guid.Empty, Guid.NewGuid(), "DISCOUNT", 10m, DiscountType.Fixed, DateTimeOffset.UtcNow.AddDays(30), 100);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "CourseId");
        }

        [Fact]
        public void Validate_EmptyCode_HasValidationError()
        {
            var command = new CreateCouponCommand(Guid.NewGuid(), Guid.NewGuid(), "", 10m, DiscountType.Fixed, DateTimeOffset.UtcNow.AddDays(30), 100);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Code");
        }

        [Fact]
        public void Validate_CodeTooShort_HasValidationError()
        {
            var command = new CreateCouponCommand(Guid.NewGuid(), Guid.NewGuid(), "AB", 10m, DiscountType.Fixed, DateTimeOffset.UtcNow.AddDays(30), 100);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Code");
        }

        [Fact]
        public void Validate_ValueZero_HasValidationError()
        {
            var command = new CreateCouponCommand(Guid.NewGuid(), Guid.NewGuid(), "DISCOUNT", 0m, DiscountType.Fixed, DateTimeOffset.UtcNow.AddDays(30), 100);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Value");
        }

        [Fact]
        public void Validate_InvalidEnum_HasValidationError()
        {
            var command = new CreateCouponCommand(Guid.NewGuid(), Guid.NewGuid(), "DISCOUNT", 10m, (DiscountType)99, DateTimeOffset.UtcNow.AddDays(30), 100);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Type");
        }

        [Fact]
        public void Validate_ExpirationDateInPast_HasValidationError()
        {
            var command = new CreateCouponCommand(Guid.NewGuid(), Guid.NewGuid(), "DISCOUNT", 10m, DiscountType.Fixed, DateTimeOffset.UtcNow.AddDays(-1), 100);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "ExpirationDate");
        }

        [Fact]
        public void Validate_UsageLimitZero_HasValidationError()
        {
            var command = new CreateCouponCommand(Guid.NewGuid(), Guid.NewGuid(), "DISCOUNT", 10m, DiscountType.Fixed, DateTimeOffset.UtcNow.AddDays(30), 0);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "UsageLimit");
        }

        [Fact]
        public void Validate_ValidCommand_NoValidationErrors()
        {
            var command = new CreateCouponCommand(Guid.NewGuid(), Guid.NewGuid(), "DISCOUNT", 10m, DiscountType.Fixed, DateTimeOffset.UtcNow.AddDays(30), 100);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeTrue();
        }
    }
}
