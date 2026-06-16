using FluentAssertions;
using MasarHub.Application.Features.Lessons.Commands.ToggleLessonPreview;

namespace MasarHub.Application.UnitTests.Features.Lessons.Commands.ToggleLessonPreview
{
    [Trait("UnitTests.Feature.Lessons", "ToggleLessonPreview")]
    public sealed class ToggleLessonPreviewCommandValidatorTests
    {
        private readonly ToggleLessonPreviewCommandValidator _sut = new();

        [Fact]
        public void Validate_ValidCommand_ReturnsNoErrors()
        {
            var command = new ToggleLessonPreviewCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), true);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_EmptyModuleId_ReturnsValidationError()
        {
            var command = new ToggleLessonPreviewCommand(Guid.Empty, Guid.NewGuid(), Guid.NewGuid(), true);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "ModuleId");
        }

        [Fact]
        public void Validate_EmptyLessonId_ReturnsValidationError()
        {
            var command = new ToggleLessonPreviewCommand(Guid.NewGuid(), Guid.Empty, Guid.NewGuid(), true);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "LessonId");
        }
    }
}
