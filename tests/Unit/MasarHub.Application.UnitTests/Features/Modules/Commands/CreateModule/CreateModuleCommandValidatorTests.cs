using FluentAssertions;
using MasarHub.Application.Features.Modules.Commands.CreateModule;

namespace MasarHub.Application.UnitTests.Features.Modules.Commands.CreateModule
{
    [Trait("UnitTests.Feature.Modules", "CreateModule")]
    public sealed class CreateModuleCommandValidatorTests
    {
        private readonly CreateModuleCommandValidator _sut = new();

        [Fact]
        public void Validate_ValidCommand_ReturnsNoErrors()
        {
            var command = new CreateModuleCommand(Guid.NewGuid(), Guid.NewGuid(), "Introduction", "Module description");

            var result = _sut.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_EmptyCourseId_ReturnsValidationError()
        {
            var command = new CreateModuleCommand(Guid.Empty, Guid.NewGuid(), "Introduction", null);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "CourseId");
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("AB")]
        public void Validate_InvalidTitle_ReturnsValidationError(string title)
        {
            var command = new CreateModuleCommand(Guid.NewGuid(), Guid.NewGuid(), title, null);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Title");
        }

        [Fact]
        public void Validate_TitleTooLong_ReturnsValidationError()
        {
            var command = new CreateModuleCommand(Guid.NewGuid(), Guid.NewGuid(), new string('A', 201), null);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorCode == "validation.max_length");
        }

        [Fact]
        public void Validate_DescriptionTooLong_ReturnsValidationError()
        {
            var command = new CreateModuleCommand(Guid.NewGuid(), Guid.NewGuid(), "Introduction", new string('A', 2001));

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorCode == "validation.max_length");
        }
    }
}
