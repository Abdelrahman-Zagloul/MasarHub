using FluentAssertions;
using MasarHub.Application.Features.Questions.Commands.UpdateQuestion;
using MasarHub.Domain.Modules.Exams;

namespace MasarHub.Application.UnitTests.Features.Questions.Commands.UpdateQuestion
{
    [Trait("UnitTests.Feature.Questions", "UpdateQuestion")]
    public sealed class UpdateQuestionCommandValidatorTests
    {
        private readonly UpdateQuestionCommandValidator _sut = new();
        private static readonly Guid InstructorId = Guid.NewGuid();
        private static readonly Guid OptionId1 = Guid.NewGuid();
        private static readonly Guid OptionId2 = Guid.NewGuid();
        private static readonly Guid OptionId3 = Guid.NewGuid();

        [Fact]
        public void Validate_ValidCommandWithAllFields_ReturnsNoErrors()
        {
            var command = new UpdateQuestionCommand(Guid.NewGuid(), Guid.NewGuid(), InstructorId, "Updated text?", 10,
            [
                new Question.OptionUpdateInput(OptionId1, "Option A", true),
                new Question.OptionUpdateInput(OptionId2, "Option B", false),
                new Question.OptionUpdateInput(OptionId3, "Option C", false)
            ]);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_ValidCommandWithOnlyRequiredFields_ReturnsNoErrors()
        {
            var command = new UpdateQuestionCommand(Guid.NewGuid(), Guid.NewGuid(), InstructorId, null, null, null);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_EmptyExamId_ReturnsValidationError()
        {
            var command = new UpdateQuestionCommand(Guid.Empty, Guid.NewGuid(), InstructorId, null, null, null);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "ExamId");
        }

        [Fact]
        public void Validate_EmptyQuestionId_ReturnsValidationError()
        {
            var command = new UpdateQuestionCommand(Guid.NewGuid(), Guid.Empty, InstructorId, null, null, null);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "QuestionId");
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Validate_QuestionTextTooShort_ReturnsValidationError(string questionText)
        {
            var command = new UpdateQuestionCommand(Guid.NewGuid(), Guid.NewGuid(), InstructorId, questionText, 10, null);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "QuestionText");
        }

        [Fact]
        public void Validate_QuestionTextTooLong_ReturnsValidationError()
        {
            var longText = new string('A', 1001);
            var command = new UpdateQuestionCommand(Guid.NewGuid(), Guid.NewGuid(), InstructorId, longText, 10, null);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorCode == "validation.max_length");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Validate_InvalidQuestionMark_ReturnsValidationError(decimal questionMark)
        {
            var command = new UpdateQuestionCommand(Guid.NewGuid(), Guid.NewGuid(), InstructorId, null, questionMark, null);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "QuestionMark");
        }

        [Fact]
        public void Validate_EmptyOptions_ReturnsValidationError()
        {
            var command = new UpdateQuestionCommand(Guid.NewGuid(), Guid.NewGuid(), InstructorId, null, null, []);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Options");
        }

        [Fact]
        public void Validate_OptionWithEmptyId_ReturnsValidationError()
        {
            var command = new UpdateQuestionCommand(Guid.NewGuid(), Guid.NewGuid(), InstructorId, null, null,
            [
                new Question.OptionUpdateInput(Guid.Empty, "some text", true)
            ]);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Options[0].OptionId");
        }

        [Fact]
        public void Validate_OptionWithEmptyText_ReturnsValidationError()
        {
            var command = new UpdateQuestionCommand(Guid.NewGuid(), Guid.NewGuid(), InstructorId, null, null,
            [
                new Question.OptionUpdateInput(Guid.NewGuid(), "", true)
            ]);

            var result = _sut.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorCode == "validation.min_length");
        }
    }
}
