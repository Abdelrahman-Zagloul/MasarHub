using MasarHub.Domain.SharedKernel;
using MasarHub.Domain.SharedKernel.Base;

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
            QuestionId = Guard.AgainstEmptyGuid(questionId, nameof(questionId));
            Text = Guard.AgainstNullOrWhiteSpace(text, nameof(text));
            IsCorrect = isCorrect;
        }

        public static Option Create(Guid questionId, string text, bool isCorrect = false)
        {
            return new Option(questionId, text, isCorrect);
        }

        public void UpdateOptionText(string optiontext)
        {
            Text = Guard.AgainstNullOrWhiteSpace(optiontext, nameof(optiontext));
            MarkAsUpdated();
        }
    }
}
