using MasarHub.Domain.Common.Base;
using MasarHub.Domain.Common.Guards;
using MasarHub.Domain.Common.Results;

namespace MasarHub.Domain.Modules.Exams
{
    public sealed class Option : BaseEntity
    {
        public Guid QuestionId { get; private set; }
        public string Text { get; private set; } = null!;
        public bool IsCorrect { get; private set; }

        private Option() { }

        private Option(Guid questionId, string text, bool isCorrect)
        {
            QuestionId = questionId;
            Text = text;
            IsCorrect = isCorrect;
        }

        public static Result<Option> Create(Guid questionId, string text, bool isCorrect = false)
        {
            var error = GuardExtensions.FirstError(
                Guard.AgainstEmptyGuid(questionId, nameof(questionId)),
                Guard.AgainstNullOrWhiteSpace(text, nameof(text))
            );

            if (error is not null)
                return error;

            return new Option(questionId, text, isCorrect);
        }

        public Result UpdateOptionText(string optiontext)
        {
            var error = Guard.AgainstNullOrWhiteSpace(optiontext, nameof(optiontext));
            if (error is not null)
                return error;

            Text = optiontext;
            MarkAsUpdated();
            return Result.Success();
        }
    }
}
