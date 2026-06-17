using FluentAssertions;
using MasarHub.Application.Features.Exams.Commands.UpdateExam;

namespace MasarHub.Application.UnitTests.Features.Exams.Commands.UpdateExam
{
    [Trait("UnitTests.Feature.Exams", "UpdateExam")]
    public sealed class UpdateExamCommandValidatorTests
    {
        private readonly UpdateExamCommandValidator _sut = new();
        private static readonly Guid InstructorId = Guid.NewGuid();

        [Fact]
        public void Validate_ValidCommand_ReturnsNoErrors()
        {
            var command = new UpdateExamCommand(Guid.NewGuid(), InstructorId, "New Title", "Description", null, 80, 90);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_EmptyExamId_ReturnsValidationError()
        {
            var command = new UpdateExamCommand(Guid.Empty, InstructorId, "New Title", null, null, null, null);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "ExamId");
        }

        [Fact]
        public void Validate_TitleTooShort_ReturnsValidationError()
        {
            var command = new UpdateExamCommand(Guid.NewGuid(), InstructorId, "AB", null, null, null, null);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorCode == "validation.min_length");
        }

        [Fact]
        public void Validate_TitleTooLong_ReturnsValidationError()
        {
            var longTitle = new string('A', 201);
            var command = new UpdateExamCommand(Guid.NewGuid(), InstructorId, longTitle, null, null, null, null);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorCode == "validation.max_length");
        }

        [Fact]
        public void Validate_DescriptionTooLong_ReturnsValidationError()
        {
            var longDesc = new string('A', 2001);
            var command = new UpdateExamCommand(Guid.NewGuid(), InstructorId, "Title", longDesc, null, null, null);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorCode == "validation.max_length");
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(101)]
        public void Validate_InvalidPassingScore_ReturnsValidationError(int score)
        {
            var command = new UpdateExamCommand(Guid.NewGuid(), InstructorId, "Title", null, null, score, null);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "PassingScorePercentage");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Validate_InvalidDuration_ReturnsValidationError(int duration)
        {
            var command = new UpdateExamCommand(Guid.NewGuid(), InstructorId, "Title", null, null, null, duration);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "DurationMinutes");
        }
    }
}
