using MasarHub.Domain.Common.Errors;
using MasarHub.Domain.Modules.Exams;

namespace MasarHub.Domain.UnitTests.Modules.Exams
{
    [Trait("UnitTests.Domain.Exams", "Option")]
public sealed class OptionTests
    {
        private static readonly Guid ValidQuestionId = Guid.NewGuid();
        private const string ValidText = "Paris";

        #region Create

        [Fact]
        public void Create_ValidInput_ReturnsSuccess()
        {
            var result = Option.Create(ValidQuestionId, ValidText);

            Assert.True(result.IsSuccess);
            Assert.Equal(ValidQuestionId, result.Value.QuestionId);
            Assert.Equal(ValidText, result.Value.Text);
            Assert.False(result.Value.IsCorrect);
        }

        [Fact]
        public void Create_WithIsCorrect_ReturnsSuccess()
        {
            var result = Option.Create(ValidQuestionId, ValidText, true);

            Assert.True(result.IsSuccess);
            Assert.True(result.Value.IsCorrect);
        }

        [Fact]
        public void Create_EmptyQuestionId_ReturnsError()
        {
            var result = Option.Create(Guid.Empty, ValidText);

            Assert.True(result.IsFailure);
            Assert.Equal(DomainError.EmptyGuid("questionId").Code, result.Error.Code);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Create_InvalidText_ReturnsError(string? text)
        {
            var result = Option.Create(ValidQuestionId, text!);

            Assert.True(result.IsFailure);
            Assert.Equal(DomainError.NullOrEmpty("text").Code, result.Error.Code);
        }

        #endregion

        #region UpdateOptionText

        [Fact]
        public void UpdateOptionText_ValidInput_ReturnsSuccess()
        {
            var option = CreateValidOption();
            var newText = "London";

            var result = option.UpdateOptionText(newText);

            Assert.True(result.IsSuccess);
            Assert.Equal(newText, option.Text);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void UpdateOptionText_InvalidInput_ReturnsError(string? text)
        {
            var option = CreateValidOption();

            var result = option.UpdateOptionText(text!);

            Assert.True(result.IsFailure);
        }

        #endregion

        #region SetIsCorrect

        [Fact]
        public void SetIsCorrect_True_UpdatesProperty()
        {
            var option = CreateValidOption();

            option.SetIsCorrect(true);

            Assert.True(option.IsCorrect);
        }

        [Fact]
        public void SetIsCorrect_False_UpdatesProperty()
        {
            var option = Option.Create(ValidQuestionId, ValidText, true).Value;

            option.SetIsCorrect(false);

            Assert.False(option.IsCorrect);
        }

        #endregion

        private static Option CreateValidOption()
        {
            return Option.Create(ValidQuestionId, ValidText).Value;
        }
    }
}