using FluentAssertions;
using MasarHub.Application.Features.Lessons.Commands.ToggleLessonArchive;

namespace MasarHub.Application.UnitTests.Features.Lessons.Commands.ToggleLessonArchive
{
    [Trait("UnitTests.Feature.Lessons", "ToggleLessonArchive")]
    public sealed class ToggleLessonArchiveCommandValidatorTests
    {
        private readonly ToggleLessonArchiveCommandValidator _sut = new();

        [Fact]
        public void Validate_ValidCommand_ReturnsNoErrors()
        {
            var command = new ToggleLessonArchiveCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), true);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_EmptyModuleId_ReturnsValidationError()
        {
            var command = new ToggleLessonArchiveCommand(Guid.Empty, Guid.NewGuid(), Guid.NewGuid(), true);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "ModuleId");
        }

        [Fact]
        public void Validate_EmptyLessonId_ReturnsValidationError()
        {
            var command = new ToggleLessonArchiveCommand(Guid.NewGuid(), Guid.Empty, Guid.NewGuid(), true);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "LessonId");
        }
    }
}
