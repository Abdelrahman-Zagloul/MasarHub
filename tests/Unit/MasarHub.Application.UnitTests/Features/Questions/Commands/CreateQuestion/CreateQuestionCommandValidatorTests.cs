using FluentAssertions;
using MasarHub.Application.Features.Questions.Commands.CreateQuestion;
using MasarHub.Domain.Modules.Exams;

namespace MasarHub.Application.UnitTests.Features.Questions.Commands.CreateQuestion
{
    [Trait("UnitTests.Feature.Questions", "CreateQuestion")]
    public sealed class CreateQuestionCommandValidatorTests
    {
        private readonly CreateQuestionCommandValidator _sut = new();
        private static readonly Guid InstructorId = Guid.NewGuid();

        [Fact]
        public void Validate_ValidCommand_ReturnsNoErrors()
        {
            var command = new CreateQuestionCommand(Guid.NewGuid(), InstructorId, "Sample question?", 10, QuestionType.SingleChoice,
            [
                new Question.OptionInput("Wrong A", false),
                new Question.OptionInput("Correct", true),
                new Question.OptionInput("Wrong B", false)
            ]);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_EmptyExamId_ReturnsValidationError()
        {
            var command = new CreateQuestionCommand(Guid.Empty, InstructorId, "Sample question?", 10, QuestionType.SingleChoice,
            [
                new Question.OptionInput("Correct", true)
            ]);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "ExamId");
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Validate_EmptyQuestionText_ReturnsValidationError(string questionText)
        {
            var command = new CreateQuestionCommand(Guid.NewGuid(), InstructorId, questionText, 10, QuestionType.SingleChoice,
            [
                new Question.OptionInput("Correct", true)
            ]);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "QuestionText");
        }

        [Fact]
        public void Validate_QuestionTextTooLong_ReturnsValidationError()
        {
            var longText = new string('A', 1001);
            var command = new CreateQuestionCommand(Guid.NewGuid(), InstructorId, longText, 10, QuestionType.SingleChoice,
            [
                new Question.OptionInput("Correct", true)
            ]);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorCode == "validation.max_length");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Validate_InvalidQuestionMark_ReturnsValidationError(decimal questionMark)
        {
            var command = new CreateQuestionCommand(Guid.NewGuid(), InstructorId, "Sample question?", questionMark, QuestionType.SingleChoice,
            [
                new Question.OptionInput("Correct", true)
            ]);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "QuestionMark");
        }

        [Fact]
        public void Validate_InvalidQuestionType_ReturnsValidationError()
        {
            var command = new CreateQuestionCommand(Guid.NewGuid(), InstructorId, "Sample question?", 10, (QuestionType)99,
            [
                new Question.OptionInput("Correct", true)
            ]);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "QuestionType");
        }

        [Fact]
        public void Validate_EmptyOptions_ReturnsValidationError()
        {
            var command = new CreateQuestionCommand(Guid.NewGuid(), InstructorId, "Sample question?", 10, QuestionType.SingleChoice,
            []);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Options");
        }

        [Fact]
        public void Validate_OptionWithEmptyText_ReturnsValidationError()
        {
            var command = new CreateQuestionCommand(Guid.NewGuid(), InstructorId, "Sample question?", 10, QuestionType.SingleChoice,
            [
                new Question.OptionInput("", true)
            ]);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorCode == "validation.required");
        }

        [Fact]
        public void Validate_DuplicateOptionText_ReturnsValidationError()
        {
            var command = new CreateQuestionCommand(Guid.NewGuid(), InstructorId, "Sample question?", 10, QuestionType.SingleChoice,
            [
                new Question.OptionInput("Same Text", true),
                new Question.OptionInput("Same Text", false),
            ]);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorCode == "validation.duplicate_option_text");
        }

        [Fact]
        public void Validate_DuplicateOptionText_DifferentCase_ReturnsValidationError()
        {
            var command = new CreateQuestionCommand(Guid.NewGuid(), InstructorId, "Sample question?", 10, QuestionType.SingleChoice,
            [
                new Question.OptionInput("Option A", true),
                new Question.OptionInput("option a", false),
            ]);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorCode == "validation.duplicate_option_text");
        }
    }
}
