using MasarHub.Domain.SharedKernel;
using MasarHub.Domain.SharedKernel.Base;

namespace MasarHub.Domain.Modules.Exams
{
    public sealed class ExamAnswer : SoftDeletableEntity
    {
        private readonly List<ExamAnswerOption> _selectedOptions = [];
        public Guid ExamAttemptId { get; private set; }
        public Guid QuestionId { get; private set; }
        public DateTimeOffset AnsweredAt { get; private set; }

        public IReadOnlyCollection<Guid> SelectedOptionIds => _selectedOptions.Select(x => x.OptionId).ToList();
        private ExamAnswer() { }

        private ExamAnswer(Guid examAttemptId, Guid questionId, IEnumerable<Guid> selectedOptionIds)
        {
            ExamAttemptId = Guard.AgainstEmptyGuid(examAttemptId, nameof(examAttemptId));
            QuestionId = Guard.AgainstEmptyGuid(questionId, nameof(questionId));
            AnsweredAt = DateTimeOffset.UtcNow;
            foreach (var selectedOptionId in selectedOptionIds)
            {
                var optionId = Guard.AgainstEmptyGuid(selectedOptionId, nameof(selectedOptionId));
                _selectedOptions.Add(ExamAnswerOption.Create(Id, optionId));
            }
        }

        public static ExamAnswer Create(Guid examAttemptId, Guid questionId, IEnumerable<Guid> selectedOptionIds)
        {
            return new ExamAnswer(examAttemptId, questionId, selectedOptionIds);
        }
    }
}
