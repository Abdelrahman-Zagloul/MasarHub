using MasarHub.Domain.Common.Base;
using MasarHub.Domain.Common.Guards;
using MasarHub.Domain.Common.Results;

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
            ExamAttemptId = examAttemptId;
            QuestionId = questionId;
            AnsweredAt = DateTimeOffset.UtcNow;

            foreach (var selectedOptionId in selectedOptionIds)
                _selectedOptions.Add(new ExamAnswerOptionBuilder(Id, selectedOptionId).Build());
        }

        public static Result<ExamAnswer> Create(Guid examAttemptId, Guid questionId, IEnumerable<Guid> selectedOptionIds)
        {
            var error = GuardExtensions.FirstError(
                Guard.AgainstEmptyGuid(examAttemptId, nameof(examAttemptId)),
                Guard.AgainstEmptyGuid(questionId, nameof(questionId)),
                Guard.AgainstNull(selectedOptionIds, nameof(selectedOptionIds))
            );

            if (error is not null)
                return error;

            var optionIds = selectedOptionIds.ToList();
            if (!optionIds.Any())
                return ExamErrors.NoAnswersSubmitted;

            foreach (var optionId in optionIds)
            {
                var optionError = Guard.AgainstEmptyGuid(optionId, nameof(selectedOptionIds));
                if (optionError is not null)
                    return optionError;
            }

            return new ExamAnswer(examAttemptId, questionId, optionIds);
        }

        public Result Delete() => MarkAsDeleted();

        private sealed class ExamAnswerOptionBuilder
        {
            private readonly Guid _examAnswerId;
            private readonly Guid _optionId;

            public ExamAnswerOptionBuilder(Guid examAnswerId, Guid optionId)
            {
                _examAnswerId = examAnswerId;
                _optionId = optionId;
            }

            public ExamAnswerOption Build()
            {
                return ExamAnswerOption.Create(_examAnswerId, _optionId).Value!;
            }
        }
    }
}
