using MasarHub.Domain.Common.Base;
using MasarHub.Domain.Common.Errors;
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
            IsPublished = false;
        }

        public static DomainResult<Exam> Create(
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
                if (moduleError != DomainError.None)
                    return moduleError;
            }

            if (durationMinutes.HasValue)
            {
                var durationError = Guard.AgainstNegativeOrZero(durationMinutes.Value, nameof(durationMinutes));
                if (durationError != DomainError.None)
                    return durationError;
            }

            if (!IsValidPassingScore(passingScorePercentage))
                return ExamErrors.InvalidPassingScore;

            return new Exam(courseId, title, passingScorePercentage, maxAttempts, moduleId, description, durationMinutes);
        }

        public DomainResult UpdateTitle(string title)
        {
            var draftResult = EnsureDraft();
            if (draftResult.IsFailure)
                return draftResult;

            var error = Guard.AgainstNullOrWhiteSpace(title, nameof(title));
            if (error != DomainError.None)
                return error;

            Title = title;
            MarkAsUpdated();
            return DomainResult.Success();
        }
        public DomainResult UpdateDescription(string? description)
        {
            var draftResult = EnsureDraft();
            if (draftResult.IsFailure)
                return draftResult;

            Description = description;
            MarkAsUpdated();
            return DomainResult.Success();
        }
        public DomainResult UpdateMaxAttempts(int maxAttempts)
        {
            var draftResult = EnsureDraft();
            if (draftResult.IsFailure)
                return draftResult;

            var error = Guard.AgainstNegativeOrZero(maxAttempts, nameof(maxAttempts));
            if (error != DomainError.None)
                return error;

            MaxAttempts = maxAttempts;
            MarkAsUpdated();
            return DomainResult.Success();
        }
        public DomainResult UpdatePassingScore(int passingScorePercentage)
        {
            var draftResult = EnsureDraft();
            if (draftResult.IsFailure)
                return draftResult;

            if (!IsValidPassingScore(passingScorePercentage))
                return ExamErrors.InvalidPassingScore;

            PassingScorePercentage = passingScorePercentage;
            MarkAsUpdated();
            return DomainResult.Success();
        }
        public DomainResult UpdateDuration(int? durationMinutes)
        {
            var draftResult = EnsureDraft();
            if (draftResult.IsFailure)
                return draftResult;

            if (durationMinutes.HasValue)
            {
                var error = Guard.AgainstNegativeOrZero(durationMinutes.Value, nameof(durationMinutes));
                if (error != DomainError.None)
                    return error;
            }

            DurationInMinutes = durationMinutes;
            MarkAsUpdated();
            return DomainResult.Success();
        }

        public DomainResult AddQuestion(Question question)
        {
            var draftResult = EnsureDraft();
            if (draftResult.IsFailure)
                return draftResult;

            var error = Guard.AgainstNull(question, nameof(question));
            if (error != DomainError.None)
                return error;

            if (question.ExamId != Id)
                return ExamErrors.InvalidQuestionExamRelation;

            _questions.Add(question);
            MarkAsUpdated();
            return DomainResult.Success();
        }

        public DomainResult Publish()
        {
            var draftResult = EnsureDraft();
            if (draftResult.IsFailure)
                return draftResult;

            if (!_questions.Any())
                return ExamErrors.MissingQuestions;

            IsPublished = true;
            MarkAsUpdated();
            return DomainResult.Success();
        }

        public DomainResult Unpublish(bool hasAttempts)
        {
            if (!IsPublished)
                return DomainResult.Success();

            if (hasAttempts)
                return ExamErrors.CannotUnpublishAfterAttempts;

            IsPublished = false;
            MarkAsUpdated();
            return DomainResult.Success();
        }

        public decimal TotalMarks() => _questions.Sum(q => q.QuestionMark);

        public decimal GetPassingScore() => TotalMarks() * PassingScorePercentage / 100m;

        public bool CanAttempt(int currentAttempts) => currentAttempts < MaxAttempts;

        public DomainResult Delete(bool hasAttempts)
        {
            var draftResult = EnsureDraft();
            if (draftResult.IsFailure)
                return draftResult;

            if (hasAttempts)
                return ExamErrors.CannotDeleteExamWithSubmissions;

            return MarkAsDeleted();
        }

        private DomainResult EnsureDraft()
        {
            return IsPublished
                ? ExamErrors.CannotModifyPublishedExam
                : DomainResult.Success();
        }

        private static bool IsValidPassingScore(int passingScorePercentage)
            => passingScorePercentage is >= 0 and <= 100;
    }
}
