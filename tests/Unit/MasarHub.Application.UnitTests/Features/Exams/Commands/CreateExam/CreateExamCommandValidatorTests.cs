using FluentAssertions;
using MasarHub.Application.Features.Exams.Commands.CreateExam;

namespace MasarHub.Application.UnitTests.Features.Exams.Commands.CreateExam
{
    [Trait("UnitTests.Feature.Exams", "CreateExam")]
    public sealed class CreateExamCommandValidatorTests
    {
        private readonly CreateExamCommandValidator _sut = new();
        private static readonly Guid InstructorId = Guid.NewGuid();

        [Fact]
        public void Validate_ValidCommand_ReturnsNoErrors()
        {
            var command = new CreateExamCommand(Guid.NewGuid(), InstructorId, "Final Exam 2026", 70, 2, null, "Description", 120);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_EmptyCourseId_ReturnsValidationError()
        {
            var command = new CreateExamCommand(Guid.Empty, InstructorId, "Final Exam", 70, 2, null, null, null);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "CourseId");
        }

        [Fact]
        public void Validate_EmptyInstructorId_ReturnsValidationError()
        {
            var command = new CreateExamCommand(Guid.NewGuid(), Guid.Empty, "Final Exam", 70, 2, null, null, null);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "InstructorId");
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Validate_EmptyTitle_ReturnsValidationError(string title)
        {
            var command = new CreateExamCommand(Guid.NewGuid(), InstructorId, title, 70, 2, null, null, null);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Title");
        }

        [Fact]
        public void Validate_TitleTooShort_ReturnsValidationError()
        {
            var command = new CreateExamCommand(Guid.NewGuid(), InstructorId, "AB", 70, 2, null, null, null);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorCode == "validation.min_length");
        }

        [Fact]
        public void Validate_TitleTooLong_ReturnsValidationError()
        {
            var longTitle = new string('A', 201);
            var command = new CreateExamCommand(Guid.NewGuid(), InstructorId, longTitle, 70, 2, null, null, null);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorCode == "validation.max_length");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(101)]
        public void Validate_InvalidPassingScore_ReturnsValidationError(int score)
        {
            var command = new CreateExamCommand(Guid.NewGuid(), InstructorId, "Final Exam", score, 2, null, null, null);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "PassingScorePercentage");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(101)]
        public void Validate_InvalidMaxAttempts_ReturnsValidationError(int attempts)
        {
            var command = new CreateExamCommand(Guid.NewGuid(), InstructorId, "Final Exam", 70, attempts, null, null, null);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "MaxAttempts");
        }

        [Fact]
        public void Validate_NegativeDuration_ReturnsValidationError()
        {
            var command = new CreateExamCommand(Guid.NewGuid(), InstructorId, "Final Exam", 70, 2, null, null, -10);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "DurationMinutes");
        }
    }
}
