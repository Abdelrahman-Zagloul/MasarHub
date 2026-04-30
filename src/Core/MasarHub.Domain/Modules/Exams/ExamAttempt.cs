using MasarHub.Domain.SharedKernel;
using MasarHub.Domain.SharedKernel.Base;
using MasarHub.Domain.SharedKernel.Exceptions;

namespace MasarHub.Domain.Modules.Exams
{
    public sealed class ExamAttempt : SoftDeletableEntity
    {
        private readonly List<ExamAnswer> _answers = new();

        public Guid ExamId { get; private set; }
        public Guid UserId { get; private set; }
        public ExamAttemptStatus Status { get; private set; }
        public DateTimeOffset StartedAt { get; private set; }
        public DateTimeOffset? SubmittedAt { get; private set; }
        public decimal Score { get; private set; }

        public IReadOnlyCollection<ExamAnswer> Answers => _answers.AsReadOnly();

        private ExamAttempt() { }

        private ExamAttempt(Guid examId, Guid userId)
        {
            ExamId = Guard.AgainstEmptyGuid(examId, nameof(examId));
            UserId = Guard.AgainstEmptyGuid(userId, nameof(userId));

            Status = ExamAttemptStatus.InProgress;
            StartedAt = DateTimeOffset.UtcNow;
        }

        public static ExamAttempt Create(Guid examId, Guid userId)
            => new(examId, userId);


        //TODO: I Will Make or Refactor When I Implement Time Limit Feature
        public void Submit(IEnumerable<Question> questions, IEnumerable<ExamAnswer> answers)
        {
            if (answers is null || !answers.Any())
                throw new DomainException("No answers submitted");

            _answers.Clear();
            _answers.AddRange(answers);

            Score = CalculateScore(questions);

            SubmittedAt = DateTimeOffset.UtcNow;
        }

        private decimal CalculateScore(IEnumerable<Question> questions)
        {
            decimal total = 0;

            foreach (var answer in _answers)
            {
                var question = questions.First(q => q.Id == answer.QuestionId);

                var correct = question.Options
                    .Where(o => o.IsCorrect)
                    .Select(o => o.Id)
                    .ToHashSet();

                var selected = answer.SelectedOptionIds.ToHashSet();

                if (selected.SetEquals(correct))
                    total += question.QuestionMark;
            }

            return total;
        }
    }
}
