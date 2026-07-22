using FluentAssertions;
using MasarHub.Application.Features.Exams.Commands.StartExamAttempt;

namespace MasarHub.Application.UnitTests.Features.Exams.Commands.StartExamAttempt
{
    [Trait("UnitTests.Feature.Exams", "StartExamAttempt")]
    public sealed class StartExamAttemptCommandValidatorTests
    {
        private readonly StartExamAttemptCommandValidator _sut = new();

        [Fact]
        public void Validate_ValidCommand_ReturnsNoErrors()
        {
            var command = new StartExamAttemptCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

            var result = _sut.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_EmptyCourseId_ReturnsValidationError()
        {
            var command = new StartExamAttemptCommand(Guid.Empty, Guid.NewGuid(), Guid.NewGuid());

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "CourseId");
        }

        [Fact]
        public void Validate_EmptyExamId_ReturnsValidationError()
        {
            var command = new StartExamAttemptCommand(Guid.NewGuid(), Guid.Empty, Guid.NewGuid());

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "ExamId");
        }

        [Fact]
        public void Validate_EmptyUserId_ReturnsValidationError()
        {
            var command = new StartExamAttemptCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.Empty);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "UserId");
        }
    }
}
