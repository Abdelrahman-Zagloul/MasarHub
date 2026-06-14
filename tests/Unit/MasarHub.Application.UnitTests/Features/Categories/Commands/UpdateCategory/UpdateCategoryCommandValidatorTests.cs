using FluentAssertions;
using MasarHub.Application.Features.Categories.Commands.UpdateCategory;
using MasarHub.Application.Features.Categories.Commands.UpdateCategoryName;

namespace MasarHub.Application.UnitTests.Features.Categories.Commands.UpdateCategory
{
    [Trait("UnitTests", "Feature.Categories.UpdateCategory.Validator")]
    public sealed class UpdateCategoryCommandValidatorTests
    {
        private readonly UpdateCategoryCommandValidator _sut = new();

        [Fact]
        public void Validate_ValidCommandWithName_ReturnsNoErrors()
        {
            var command = new UpdateCategoryCommand(Guid.NewGuid(), "NewName", null, null, false);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_ValidCommandWithMoveToRoot_ReturnsNoErrors()
        {
            var command = new UpdateCategoryCommand(Guid.NewGuid(), null, null, null, true);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_EmptyId_ReturnsValidationError()
        {
            var command = new UpdateCategoryCommand(Guid.Empty, "Name", null, null, false);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorCode == "validation.invalid_guid");
        }

        [Fact]
        public void Validate_NameTooShort_ReturnsValidationError()
        {
            var command = new UpdateCategoryCommand(Guid.NewGuid(), "AB", null, null, false);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorCode == "validation.min_length");
        }

        [Fact]
        public void Validate_NameTooLong_ReturnsValidationError()
        {
            var longName = new string('A', 201);
            var command = new UpdateCategoryCommand(Guid.NewGuid(), longName, null, null, false);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorCode == "validation.max_length");
        }

        [Fact]
        public void Validate_EmptyParentCategoryId_ReturnsValidationError()
        {
            var command = new UpdateCategoryCommand(Guid.NewGuid(), null, null, Guid.Empty, false);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorCode == "validation.invalid_guid");
        }

        [Fact]
        public void Validate_NoFieldsSpecified_ReturnsValidationError()
        {
            var command = new UpdateCategoryCommand(Guid.NewGuid(), null, null, null, false);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorCode == "validation.at_least_one_field_required");
        }

        [Fact]
        public void Validate_MoveToRootAndParentIdBothSpecified_ReturnsValidationError()
        {
            var command = new UpdateCategoryCommand(Guid.NewGuid(), null, null, Guid.NewGuid(), true);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorCode == "validation.cannot_use_parent_and_move_to_root");
        }
    }
}
