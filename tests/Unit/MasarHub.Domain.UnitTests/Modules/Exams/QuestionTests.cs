using MasarHub.Domain.Common.Errors;
using MasarHub.Domain.Modules.Exams;

namespace MasarHub.Domain.UnitTests.Modules.Exams
{
    [Trait("UnitTests.Domain.Exams", "Question")]
public sealed class QuestionTests
    {
        private static readonly Guid ValidExamId = Guid.NewGuid();
        private const string ValidText = "What is the capital of France?";
        private const decimal ValidMark = 10m;

        private static Question.OptionInput Option(string text, bool isCorrect = false)
            => new(text, isCorrect);

        #region Create

        [Fact]
        public void Create_SingleChoice_ReturnsSuccess()
        {
            var options = new[]
            {
                Option("Paris", true),
                Option("London"),
                Option("Berlin"),
                Option("Madrid"),
            };

            var result = Question.Create(ValidExamId, ValidText, ValidMark, QuestionType.SingleChoice, options);

            Assert.True(result.IsSuccess);
            Assert.Equal(ValidExamId, result.Value.ExamId);
            Assert.Equal(ValidText, result.Value.QuestionText);
            Assert.Equal(ValidMark, result.Value.QuestionMark);
            Assert.Equal(QuestionType.SingleChoice, result.Value.QuestionType);
            Assert.Equal(4, result.Value.Options.Count);
            Assert.Single(result.Value.Options, o => o.IsCorrect);
        }

        [Fact]
        public void Create_MultipleChoice_ReturnsSuccess()
        {
            var options = new[]
            {
                Option("Red", true),
                Option("Blue", true),
                Option("Green"),
                Option("Yellow"),
            };

            var result = Question.Create(ValidExamId, ValidText, ValidMark, QuestionType.MultipleChoice, options);

            Assert.True(result.IsSuccess);
            Assert.Equal(2, result.Value.Options.Count(o => o.IsCorrect));
        }

        [Fact]
        public void Create_TrueFalse_ReturnsSuccess()
        {
            var options = new[]
            {
                Option("True", true),
                Option("False"),
            };

            var result = Question.Create(ValidExamId, ValidText, ValidMark, QuestionType.TrueFalse, options);

            Assert.True(result.IsSuccess);
            Assert.Equal(2, result.Value.Options.Count);
        }

        [Fact]
        public void Create_EmptyExamId_ReturnsError()
        {
            var options = new[] { Option("Yes", true), Option("No") };

            var result = Question.Create(Guid.Empty, ValidText, ValidMark, QuestionType.TrueFalse, options);

            Assert.True(result.IsFailure);
            Assert.Equal(DomainError.EmptyGuid("examId").Code, result.Error.Code);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Create_InvalidQuestionText_ReturnsError(string? text)
        {
            var options = new[] { Option("Yes", true), Option("No") };

            var result = Question.Create(ValidExamId, text!, ValidMark, QuestionType.TrueFalse, options);

            Assert.True(result.IsFailure);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Create_InvalidMark_ReturnsError(decimal mark)
        {
            var options = new[] { Option("Yes", true), Option("No") };

            var result = Question.Create(ValidExamId, ValidText, mark, QuestionType.TrueFalse, options);

            Assert.True(result.IsFailure);
        }

        [Fact]
        public void Create_InvalidQuestionType_ReturnsError()
        {
            var options = new[] { Option("Yes", true), Option("No") };

            var result = Question.Create(ValidExamId, ValidText, ValidMark, (QuestionType)99, options);

            Assert.True(result.IsFailure);
        }

        [Fact]
        public void Create_NullOptions_ReturnsError()
        {
            var result = Question.Create(ValidExamId, ValidText, ValidMark, QuestionType.TrueFalse, null!);

            Assert.True(result.IsFailure);
        }

        [Fact]
        public void Create_EmptyOptions_ReturnsError()
        {
            var result = Question.Create(ValidExamId, ValidText, ValidMark, QuestionType.TrueFalse, []);

            Assert.True(result.IsFailure);
            Assert.Equal(ExamErrors.QuestionMustHaveOptions.Code, result.Error.Code);
        }

        [Fact]
        public void Create_TrueFalseWithMoreThanTwoOptions_ReturnsError()
        {
            var options = new[] { Option("A", true), Option("B"), Option("C") };

            var result = Question.Create(ValidExamId, ValidText, ValidMark, QuestionType.TrueFalse, options);

            Assert.True(result.IsFailure);
            Assert.Equal(ExamErrors.TrueFalseMustHaveTwoOptions.Code, result.Error.Code);
        }

        [Fact]
        public void Create_TrueFalseWithoutExactlyOneCorrect_ReturnsError()
        {
            var options = new[] { Option("True"), Option("False") };

            var result = Question.Create(ValidExamId, ValidText, ValidMark, QuestionType.TrueFalse, options);

            Assert.True(result.IsFailure);
            Assert.Equal(ExamErrors.TrueFalseMustHaveOneCorrect.Code, result.Error.Code);
        }

        [Fact]
        public void Create_SingleChoiceWithLessThan3Options_ReturnsError()
        {
            var options = new[] { Option("A", true), Option("B") };

            var result = Question.Create(ValidExamId, ValidText, ValidMark, QuestionType.SingleChoice, options);

            Assert.True(result.IsFailure);
            Assert.Equal(ExamErrors.SingleChoiceMustHaveBetween3And6.Code, result.Error.Code);
        }

        [Fact]
        public void Create_SingleChoiceWithMoreThan6Options_ReturnsError()
        {
            var options = Enumerable.Range(1, 7).Select(i => Option($"Option {i}", i == 1)).ToArray();

            var result = Question.Create(ValidExamId, ValidText, ValidMark, QuestionType.SingleChoice, options);

            Assert.True(result.IsFailure);
            Assert.Equal(ExamErrors.SingleChoiceMustHaveBetween3And6.Code, result.Error.Code);
        }

        [Fact]
        public void Create_SingleChoiceWithoutExactlyOneCorrect_ReturnsError()
        {
            var options = new[] { Option("A"), Option("B"), Option("C") };

            var result = Question.Create(ValidExamId, ValidText, ValidMark, QuestionType.SingleChoice, options);

            Assert.True(result.IsFailure);
            Assert.Equal(ExamErrors.SingleChoiceMustHaveOneCorrect.Code, result.Error.Code);
        }

        [Fact]
        public void Create_MultipleChoiceWithLessThan3Options_ReturnsError()
        {
            var options = new[] { Option("A", true), Option("B", true) };

            var result = Question.Create(ValidExamId, ValidText, ValidMark, QuestionType.MultipleChoice, options);

            Assert.True(result.IsFailure);
            Assert.Equal(ExamErrors.MultipleChoiceMustHaveBetween3And10.Code, result.Error.Code);
        }

        [Fact]
        public void Create_MultipleChoiceWithMoreThan10Options_ReturnsError()
        {
            var options = Enumerable.Range(1, 11).Select(i => Option($"Option {i}", i <= 2)).ToArray();

            var result = Question.Create(ValidExamId, ValidText, ValidMark, QuestionType.MultipleChoice, options);

            Assert.True(result.IsFailure);
            Assert.Equal(ExamErrors.MultipleChoiceMustHaveBetween3And10.Code, result.Error.Code);
        }

        [Fact]
        public void Create_MultipleChoiceWithLessThan2Correct_ReturnsError()
        {
            var options = new[] { Option("A", true), Option("B"), Option("C") };

            var result = Question.Create(ValidExamId, ValidText, ValidMark, QuestionType.MultipleChoice, options);

            Assert.True(result.IsFailure);
            Assert.Equal(ExamErrors.MultipleChoiceMustHaveAtLeastTwoCorrect.Code, result.Error.Code);
        }

        [Fact]
        public void Create_DuplicateOptionText_ReturnsError()
        {
            var options = new[]
            {
                Option("Paris", true),
                Option("Paris"),
                Option("London"),
            };

            var result = Question.Create(ValidExamId, ValidText, ValidMark, QuestionType.SingleChoice, options);

            Assert.True(result.IsFailure);
            Assert.Equal(ExamErrors.DuplicateOptionText.Code, result.Error.Code);
        }

        #endregion

        #region UpdateQuestionText

        [Fact]
        public void UpdateQuestionText_ValidInput_ReturnsSuccess()
        {
            var question = CreateValidSingleChoice();
            var newText = "Updated question?";

            var result = question.UpdateQuestionText(newText);

            Assert.True(result.IsSuccess);
            Assert.Equal(newText, question.QuestionText);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void UpdateQuestionText_InvalidInput_ReturnsError(string? text)
        {
            var question = CreateValidSingleChoice();

            var result = question.UpdateQuestionText(text!);

            Assert.True(result.IsFailure);
        }

        #endregion

        #region UpdateQuestionMark

        [Fact]
        public void UpdateQuestionMark_ValidInput_ReturnsSuccess()
        {
            var question = CreateValidSingleChoice();
            var newMark = 20m;

            var result = question.UpdateQuestionMark(newMark);

            Assert.True(result.IsSuccess);
            Assert.Equal(newMark, question.QuestionMark);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-5)]
        public void UpdateQuestionMark_InvalidInput_ReturnsError(decimal mark)
        {
            var question = CreateValidSingleChoice();

            var result = question.UpdateQuestionMark(mark);

            Assert.True(result.IsFailure);
        }

        #endregion

        #region UpdateOptions

        [Fact]
        public void UpdateOptions_ValidInput_ReturnsSuccess()
        {
            var question = CreateValidSingleChoice();
            var option = question.Options.First();
            var newText = "New York";

            var updates = new[] { new Question.OptionUpdateInput(option.Id, newText, null) };
            var result = question.UpdateOptions(updates);

            Assert.True(result.IsSuccess);
            Assert.Equal(newText, option.Text);
        }

        [Fact]
        public void UpdateOptions_EmptyInput_ReturnsError()
        {
            var question = CreateValidSingleChoice();

            var result = question.UpdateOptions([]);

            Assert.True(result.IsFailure);
            Assert.Equal(ExamErrors.QuestionMustHaveOptions.Code, result.Error.Code);
        }

        [Fact]
        public void UpdateOptions_OptionNotFound_ReturnsError()
        {
            var question = CreateValidSingleChoice();

            var updates = new[] { new Question.OptionUpdateInput(Guid.NewGuid(), "New text", null) };
            var result = question.UpdateOptions(updates);

            Assert.True(result.IsFailure);
        }

        [Fact]
        public void UpdateOptions_InvalidOptionText_ReturnsError()
        {
            var question = CreateValidSingleChoice();
            var option = question.Options.First();

            var updates = new[] { new Question.OptionUpdateInput(option.Id, "", null) };
            var result = question.UpdateOptions(updates);

            Assert.True(result.IsFailure);
        }

        #endregion

        private static Question CreateValidSingleChoice()
        {
            var options = new[]
            {
                new Question.OptionInput("Paris", true),
                new Question.OptionInput("London", false),
                new Question.OptionInput("Berlin", false),
                new Question.OptionInput("Madrid", false),
            };

            return Question.Create(ValidExamId, ValidText, ValidMark, QuestionType.SingleChoice, options).Value;
        }
    }
}