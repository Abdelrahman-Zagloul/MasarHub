using MasarHub.Domain.Common.Base;
using MasarHub.Domain.Common.Guards;
using MasarHub.Domain.Common.Results;

namespace MasarHub.Domain.Modules.Exams
{
    public sealed class Exam : SoftDeletableEntity
    {
        private readonly List<Question> _questions = [];

        public Guid CourseId { get; private set; }
        public Guid? ModuleId { get; private set; }
        public string Title { get; private set; } = null!;
        public string? Description { get; private set; }
        public int PassingScorePercentage { get; private set; }
        public int? DurationInMinutes { get; private set; }
        public int MaxAttempts { get; private set; }
        public bool IsPublished { get; private set; }
        public IReadOnlyCollection<Question> Questions => _questions.AsReadOnly();

        private Exam() { }

        private Exam(
            Guid courseId,
            string title,
            int passingScorePercentage,
            int maxAttempts,
            Guid? moduleId,
            string? description,
            int? durationMinutes)
        {
            CourseId = courseId;
            Title = title;
            PassingScorePercentage = passingScorePercentage;
            MaxAttempts = maxAttempts;
            ModuleId = moduleId;
            Description = description;
            DurationInMinutes = durationMinutes;
        }

        public static Result<Exam> Create(
            Guid courseId,
            string title,
            int passingScorePercentage,
            int maxAttempts,
            Guid? moduleId = null,
            string? description = null,
            int? durationMinutes = null)
        {
            var error = GuardExtensions.FirstError(
                Guard.AgainstEmptyGuid(courseId, nameof(courseId)),
                Guard.AgainstNullOrWhiteSpace(title, nameof(title)),
                Guard.AgainstNegativeOrZero(maxAttempts, nameof(maxAttempts))
            );
            if (error is not null)
                return error;

            if (moduleId.HasValue)
            {
                var moduleError = Guard.AgainstEmptyGuid(moduleId.Value, nameof(moduleId));
                if (moduleError is not null)
                    return moduleError;
            }

            if (durationMinutes.HasValue)
            {
                var durationError = Guard.AgainstNegativeOrZero(durationMinutes.Value, nameof(durationMinutes));
                if (durationError is not null)
                    return durationError;
            }

            if (!IsValidPassingScore(passingScorePercentage))
                return ExamErrors.InvalidPassingScore;

            return new Exam(courseId, title, passingScorePercentage, maxAttempts, moduleId, description, durationMinutes);
        }

        public Result UpdateTitle(string title)
        {
            var draftResult = EnsureDraft();
            if (draftResult.IsFailure)
                return draftResult;

            var error = Guard.AgainstNullOrWhiteSpace(title, nameof(title));
            if (error is not null)
                return error;

            Title = title;
            MarkAsUpdated();
            return Result.Success();
        }

        public Result UpdateDescription(string? description)
        {
            var draftResult = EnsureDraft();
            if (draftResult.IsFailure)
                return draftResult;

            Description = description;
            MarkAsUpdated();
            return Result.Success();
        }

        public Result SetPassingScore(int passingScorePercentage)
        {
            var draftResult = EnsureDraft();
            if (draftResult.IsFailure)
                return draftResult;

            if (!IsValidPassingScore(passingScorePercentage))
                return ExamErrors.InvalidPassingScore;

            PassingScorePercentage = passingScorePercentage;
            MarkAsUpdated();
            return Result.Success();
        }

        public Result SetDuration(int? durationMinutes)
        {
            var draftResult = EnsureDraft();
            if (draftResult.IsFailure)
                return draftResult;

            if (durationMinutes.HasValue)
            {
                var error = Guard.AgainstNegativeOrZero(durationMinutes.Value, nameof(durationMinutes));
                if (error is not null)
                    return error;
            }

            DurationInMinutes = durationMinutes;
            MarkAsUpdated();
            return Result.Success();
        }

        public Result AddQuestion(Question question)
        {
            var draftResult = EnsureDraft();
            if (draftResult.IsFailure)
                return draftResult;

            var error = Guard.AgainstNull(question, nameof(question));
            if (error is not null)
                return error;

            if (question.ExamId != Id)
                return ExamErrors.InvalidQuestionExamRelation;

            _questions.Add(question);
            MarkAsUpdated();
            return Result.Success();
        }

        public Result Publish()
        {
            var draftResult = EnsureDraft();
            if (draftResult.IsFailure)
                return draftResult;

            if (!_questions.Any())
                return ExamErrors.MissingQuestions;

            IsPublished = true;
            MarkAsUpdated();
            return Result.Success();
        }

        public Result Unpublish(bool hasSubmissions)
        {
            if (!IsPublished)
                return Result.Success();

            if (hasSubmissions)
                return ExamErrors.CannotUnpublishAfterAttempts;

            IsPublished = false;
            MarkAsUpdated();
            return Result.Success();
        }

        public decimal TotalMarks() => _questions.Sum(q => q.QuestionMark);

        public decimal GetPassingScore() => TotalMarks() * PassingScorePercentage / 100m;

        public bool CanAttempt(int currentAttempts) => currentAttempts < MaxAttempts;

        public Result Delete() => MarkAsDeleted();

        private Result EnsureDraft()
        {
            return IsPublished
                ? ExamErrors.CannotModifyPublishedExam
                : Result.Success();
        }

        private static bool IsValidPassingScore(int passingScorePercentage)
            => passingScorePercentage is >= 0 and <= 100;
    }
}
