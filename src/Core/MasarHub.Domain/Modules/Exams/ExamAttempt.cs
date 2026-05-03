using MasarHub.Domain.Common.Base;
using MasarHub.Domain.Common.Guards;
using MasarHub.Domain.Common.Results;

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
            ExamId = examId;
            UserId = userId;
            Status = ExamAttemptStatus.InProgress;
            StartedAt = DateTimeOffset.UtcNow;
        }

        public static Result<ExamAttempt> Create(Guid examId, Guid userId)
        {
            var error = GuardExtensions.FirstError(
                Guard.AgainstEmptyGuid(examId, nameof(examId)),
                Guard.AgainstEmptyGuid(userId, nameof(userId))
            );

            if (error is not null)
                return error;

            return new ExamAttempt(examId, userId);
        }

        public Result Submit(IEnumerable<Question> questions, IEnumerable<ExamAnswer> answers)
        {
            var error = GuardExtensions.FirstError(
                Guard.AgainstNull(questions, nameof(questions)),
                Guard.AgainstNull(answers, nameof(answers))
            );
            if (error is not null)
                return error;

            var answerList = answers.ToList();
            if (!answerList.Any())
                return ExamErrors.NoAnswersSubmitted;

            _answers.Clear();
            _answers.AddRange(answerList);
            Score = CalculateScore(questions);
            Status = ExamAttemptStatus.Submitted;
            SubmittedAt = DateTimeOffset.UtcNow;
            MarkAsUpdated();

            return Result.Success();
        }

        public Result Delete() => MarkAsDeleted();

        private decimal CalculateScore(IEnumerable<Question> questions)
        {
            decimal total = 0;
            var questionList = questions.ToList();

            foreach (var answer in _answers)
            {
                var question = questionList.First(q => q.Id == answer.QuestionId);

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
