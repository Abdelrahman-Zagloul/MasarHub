using MasarHub.Domain.Common.Base;
using MasarHub.Domain.Common.Errors;
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

        public static DomainResult<Option> Create(Guid questionId, string text, bool isCorrect = false)
        {
            var error = GuardExtensions.FirstError(
                Guard.AgainstEmptyGuid(questionId, nameof(questionId)),
                Guard.AgainstNullOrWhiteSpace(text, nameof(text))
            );

            if (error is not null)
                return error;

            return new Option(questionId, text, isCorrect);
        }

        public DomainResult UpdateOptionText(string optionText)
        {
            var error = Guard.AgainstNullOrWhiteSpace(optionText, nameof(optionText));
            if (error != DomainError.None)
                return error;

            Text = optionText;
            MarkAsUpdated();
            return DomainResult.Success();
        }
    }
}
