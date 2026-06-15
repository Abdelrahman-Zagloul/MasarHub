using FluentAssertions;
using MasarHub.Application.Features.Modules.Commands.UpdateModule;

namespace MasarHub.Application.UnitTests.Features.Modules.Commands.UpdateModule
{
    [Trait("UnitTests.Feature.Modules", "UpdateModule")]
    public sealed class UpdateModuleCommandValidatorTests
    {
        private readonly UpdateModuleCommandValidator _sut = new();

        [Fact]
        public void Validate_ValidCommand_ReturnsNoErrors()
        {
            var command = new UpdateModuleCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "New Title", null);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_EmptyModuleId_ReturnsValidationError()
        {
            var command = new UpdateModuleCommand(Guid.NewGuid(), Guid.Empty, Guid.NewGuid(), "New Title", null);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "ModuleId");
        }

        [Fact]
        public void Validate_TitleTooShort_ReturnsValidationError()
        {
            var command = new UpdateModuleCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "AB", null);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorCode == "validation.min_length");
        }

        [Fact]
        public void Validate_TitleTooLong_ReturnsValidationError()
        {
            var longTitle = new string('A', 201);
            var command = new UpdateModuleCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), longTitle, null);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorCode == "validation.max_length");
        }

        [Fact]
        public void Validate_DescriptionTooLong_ReturnsValidationError()
        {
            var longDesc = new string('A', 2001);
            var command = new UpdateModuleCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Title", longDesc);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorCode == "validation.max_length");
        }
    }
}
