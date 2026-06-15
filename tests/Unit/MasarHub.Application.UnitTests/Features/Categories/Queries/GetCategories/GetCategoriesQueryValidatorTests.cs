using FluentAssertions;
using MasarHub.Application.Features.Categories.Queries.GetCategories;

namespace MasarHub.Application.UnitTests.Features.Categories.Queries.GetCategories
{
    [Trait("UnitTests.Feature.Categories", "GetCategories")]
    public sealed class GetCategoriesQueryValidatorTests
    {
        private readonly GetCategoriesQueryValidator _sut = new();

        [Fact]
        public void Validate_ValidQuery_ReturnsNoErrors()
        {
            var query = new GetCategoriesQuery(null, null, 1, 10);

            var result = _sut.Validate(query);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_InvalidPageNumber_ReturnsValidationError()
        {
            var query = new GetCategoriesQuery(null, null, 0, 10);

            var result = _sut.Validate(query);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorCode == "validation.page_number_invalid");
        }

        [Fact]
        public void Validate_InvalidPageSize_ReturnsValidationError()
        {
            var query = new GetCategoriesQuery(null, null, 1, 0);

            var result = _sut.Validate(query);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorCode == "validation.page_size_invalid");
        }

        [Fact]
        public void Validate_PageSizeTooLarge_ReturnsValidationError()
        {
            var query = new GetCategoriesQuery(null, null, 1, 51);

            var result = _sut.Validate(query);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorCode == "validation.page_size_too_large");
        }

        [Fact]
        public void Validate_InvalidLevelBelowRange_ReturnsValidationError()
        {
            var query = new GetCategoriesQuery(null, 0, 1, 10);

            var result = _sut.Validate(query);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorCode == "category.invalid_level");
        }

        [Fact]
        public void Validate_InvalidLevelAboveRange_ReturnsValidationError()
        {
            var query = new GetCategoriesQuery(null, 4, 1, 10);

            var result = _sut.Validate(query);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorCode == "category.invalid_level");
        }

        [Fact]
        public void Validate_NullLevel_ReturnsNoErrors()
        {
            var query = new GetCategoriesQuery(null, null, 1, 10);

            var result = _sut.Validate(query);

            result.IsValid.Should().BeTrue();
        }
    }
}
