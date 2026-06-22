using FluentAssertions;
using MasarHub.Application.Features.Coupons.Queries.GetCoupons;
using MasarHub.Domain.Modules.Payments;

namespace MasarHub.Application.UnitTests.Features.Coupons.Queries.GetCoupons
{
    [Trait("UnitTests.Feature.Coupons", "GetCoupons")]
    public sealed class GetCouponsQueryValidatorTests
    {
        private readonly GetCouponsQueryValidator _sut = new();

        [Fact]
        public void Validate_EmptyCourseId_HasValidationError()
        {
            var query = new GetCouponsQuery(Guid.Empty, Guid.NewGuid());

            var result = _sut.Validate(query);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "CourseId");
        }

        [Fact]
        public void Validate_InvalidStatus_HasValidationError()
        {
            var query = new GetCouponsQuery(Guid.NewGuid(), Guid.NewGuid(), (CouponStatus)99);

            var result = _sut.Validate(query);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Status");
        }

        [Fact]
        public void Validate_ValidQuery_NoValidationErrors()
        {
            var query = new GetCouponsQuery(Guid.NewGuid(), Guid.NewGuid());

            var result = _sut.Validate(query);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_ValidQueryWithStatus_NoValidationErrors()
        {
            var query = new GetCouponsQuery(Guid.NewGuid(), Guid.NewGuid(), CouponStatus.Active);

            var result = _sut.Validate(query);

            result.IsValid.Should().BeTrue();
        }
    }
}
