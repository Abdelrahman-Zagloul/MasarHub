using FluentAssertions;
using MasarHub.Application.Features.Categories.Commands.CreateCategory;

namespace MasarHub.Application.UnitTests.Features.Categories.Commands.CreateCategory
{
    [Trait("UnitTests", "Feature.Categories.CreateCategory.Validator")]
    public sealed class CreateCategoryCommandValidatorTests
    {
        private readonly CreateCategoryCommandValidator _sut = new();

        [Fact]
        public void Validate_ValidCommand_ReturnsNoErrors()
        {
            var command = new CreateCategoryCommand("Programming", "Description", null);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Validate_EmptyName_ReturnsValidationError(string name)
        {
            var command = new CreateCategoryCommand(name, null, null);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Name");
        }

        [Fact]
        public void Validate_NameTooShort_ReturnsValidationError()
        {
            var command = new CreateCategoryCommand("AB", null, null);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorCode == "validation.min_length");
        }

        [Fact]
        public void Validate_NameTooLong_ReturnsValidationError()
        {
            var longName = new string('A', 201);
            var command = new CreateCategoryCommand(longName, null, null);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorCode == "validation.max_length");
        }
    }
}
