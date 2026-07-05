using FluentAssertions;
using MasarHub.Application.Features.Progress.Commands.CompleteLesson;

namespace MasarHub.Application.UnitTests.Features.Progress.Commands.CompleteLesson
{
    [Trait("UnitTests.Feature.Progress", "CompleteLesson")]
    public sealed class CompleteLessonCommandValidatorTests
    {
        private readonly CompleteLessonCommandValidator _sut = new();

        [Fact]
        public void Validate_ValidCommand_ReturnsNoErrors()
        {
            var command = new CompleteLessonCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

            var result = _sut.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_EmptyUserId_ReturnsValidationError()
        {
            var command = new CompleteLessonCommand(Guid.Empty, Guid.NewGuid(), Guid.NewGuid());

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "UserId");
        }

        [Fact]
        public void Validate_EmptyCourseId_ReturnsValidationError()
        {
            var command = new CompleteLessonCommand(Guid.NewGuid(), Guid.Empty, Guid.NewGuid());

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "CourseId");
        }

        [Fact]
        public void Validate_EmptyLessonId_ReturnsValidationError()
        {
            var command = new CompleteLessonCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.Empty);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "LessonId");
        }
    }
}
