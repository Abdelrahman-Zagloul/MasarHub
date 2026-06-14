using FluentAssertions;
using MasarHub.Application.Features.Categories.Commands.ReorderCategories;

namespace MasarHub.Application.UnitTests.Features.Categories.Commands.ReorderCategories
{
    [Trait("UnitTests", "Feature.Categories.ReorderCategories.Validator")]
    public sealed class ReorderCategoriesCommandValidatorTests
    {
        private readonly ReorderCategoriesCommandValidator _sut = new();

        [Fact]
        public void Validate_ValidCommand_ReturnsNoErrors()
        {
            var command = new ReorderCategoriesCommand(null, new List<Guid> { Guid.NewGuid(), Guid.NewGuid() });

            var result = _sut.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_ValidCommandWithParentId_ReturnsNoErrors()
        {
            var command = new ReorderCategoriesCommand(Guid.NewGuid(), new List<Guid> { Guid.NewGuid() });

            var result = _sut.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_EmptyOrderedCategoryIds_ReturnsValidationError()
        {
            var command = new ReorderCategoriesCommand(null, new List<Guid>());

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorCode == "validation.required");
        }

        [Fact]
        public void Validate_DuplicateCategoryIds_ReturnsValidationError()
        {
            var id = Guid.NewGuid();
            var command = new ReorderCategoriesCommand(null, new List<Guid> { id, id });

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorCode == "category.duplicate_reorder_category_ids");
        }

        [Fact]
        public void Validate_EmptyGuidInOrderedIds_ReturnsValidationError()
        {
            var command = new ReorderCategoriesCommand(null, new List<Guid> { Guid.Empty });

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorCode == "category.invalid_category_ids");
        }

        [Fact]
        public void Validate_EmptyParentCategoryId_ReturnsValidationError()
        {
            var command = new ReorderCategoriesCommand(Guid.Empty, new List<Guid> { Guid.NewGuid() });

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorCode == "validation.invalid_guid");
        }
    }
}
