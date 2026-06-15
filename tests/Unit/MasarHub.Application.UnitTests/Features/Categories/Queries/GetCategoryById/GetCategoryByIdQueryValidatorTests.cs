using FluentAssertions;
using MasarHub.Application.Features.Categories.Queries.GetCategoryById;

namespace MasarHub.Application.UnitTests.Features.Categories.Queries.GetCategoryById
{
    [Trait("UnitTests.Feature.Categories", "GetCategoryById")]
    public sealed class GetCategoryByIdQueryValidatorTests
    {
        private readonly GetCategoryByIdQueryValidator _sut = new();

        [Fact]
        public void Validate_ValidQuery_ReturnsNoErrors()
        {
            var query = new GetCategoryByIdQuery(Guid.NewGuid());

            var result = _sut.Validate(query);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_EmptyId_ReturnsValidationError()
        {
            var query = new GetCategoryByIdQuery(Guid.Empty);

            var result = _sut.Validate(query);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorCode == "validation.invalid_guid");
        }
    }
}
