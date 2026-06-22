using FluentAssertions;
using MasarHub.Application.Features.Coupons.Queries.GetCouponById;

namespace MasarHub.Application.UnitTests.Features.Coupons.Queries.GetCouponById
{
    [Trait("UnitTests.Feature.Coupons", "GetCouponById")]
    public sealed class GetCouponByIdQueryValidatorTests
    {
        private readonly GetCouponByIdQueryValidator _sut = new();

        [Fact]
        public void Validate_EmptyCourseId_HasValidationError()
        {
            var query = new GetCouponByIdQuery(Guid.Empty, Guid.NewGuid(), Guid.NewGuid());

            var result = _sut.Validate(query);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "CourseId");
        }

        [Fact]
        public void Validate_EmptyCouponId_HasValidationError()
        {
            var query = new GetCouponByIdQuery(Guid.NewGuid(), Guid.Empty, Guid.NewGuid());

            var result = _sut.Validate(query);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "CouponId");
        }

        [Fact]
        public void Validate_ValidQuery_NoValidationErrors()
        {
            var query = new GetCouponByIdQuery(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

            var result = _sut.Validate(query);

            result.IsValid.Should().BeTrue();
        }
    }
}
