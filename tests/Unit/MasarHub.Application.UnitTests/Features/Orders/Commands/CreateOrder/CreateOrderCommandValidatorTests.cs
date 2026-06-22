using FluentAssertions;
using MasarHub.Application.Features.Orders.Commands.CreateOrder;

namespace MasarHub.Application.UnitTests.Features.Orders.Commands.CreateOrder
{
    [Trait("UnitTests.Feature.Orders", "CreateOrder")]
    public sealed class CreateOrderCommandValidatorTests
    {
        private readonly CreateOrderCommandValidator _sut = new();

        [Fact]
        public void Validate_NullCoupons_NoValidationError()
        {
            var command = new CreateOrderCommand(Guid.NewGuid(), null);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_EmptyCouponCode_HasValidationError()
        {
            var coupons = new List<CourseCoupon> { new("", Guid.NewGuid()) };
            var command = new CreateOrderCommand(Guid.NewGuid(), coupons);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Coupons[0].CouponCode");
        }

        [Fact]
        public void Validate_EmptyCourseId_HasValidationError()
        {
            var coupons = new List<CourseCoupon> { new("SUMMER", Guid.Empty) };
            var command = new CreateOrderCommand(Guid.NewGuid(), coupons);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Coupons[0].CourseId");
        }

        [Fact]
        public void Validate_DuplicateCourseIds_HasValidationError()
        {
            var courseId = Guid.NewGuid();
            var coupons = new List<CourseCoupon>
            {
                new("SUMMER", courseId),
                new("WINTER", courseId)
            };
            var command = new CreateOrderCommand(Guid.NewGuid(), coupons);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorCode == "validation.duplicate_course_coupon");
        }

        [Fact]
        public void Validate_ValidCommand_NoValidationErrors()
        {
            var coupons = new List<CourseCoupon>
            {
                new("SUMMER", Guid.NewGuid()),
                new("WINTER", Guid.NewGuid())
            };
            var command = new CreateOrderCommand(Guid.NewGuid(), coupons);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeTrue();
        }
    }
}
