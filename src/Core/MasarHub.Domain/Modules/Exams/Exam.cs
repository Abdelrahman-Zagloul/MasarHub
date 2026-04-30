using MasarHub.Domain.SharedKernel;
using MasarHub.Domain.SharedKernel.Base;
using MasarHub.Domain.SharedKernel.Exceptions;

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
            CourseId = Guard.AgainstEmptyGuid(courseId, nameof(courseId));
            Title = Guard.AgainstNullOrWhiteSpace(title, nameof(title));
            MaxAttempts = Guard.AgainstNegativeOrZero(maxAttempts, nameof(maxAttempts));

            ModuleId = moduleId.HasValue
                ? Guard.AgainstEmptyGuid(moduleId.Value, nameof(moduleId))
                : null;
            Description = description;
            SetPassingScore(passingScorePercentage);
            SetDuration(durationMinutes);
        }

        public static Exam Create(
            Guid courseId,
            string title,
            int passingScorePercentage,
            int maxAttempts,
            Guid? moduleId = null,
            string? description = null,
            int? durationMinutes = null)
        {
            return new Exam(courseId, title, passingScorePercentage, maxAttempts, moduleId, description, durationMinutes);
        }

        public void UpdateTitle(string title)
        {
            EnsureDraft();
            Title = Guard.AgainstNullOrWhiteSpace(title, nameof(title));
            MarkAsUpdated();
        }
        public void UpdateDescription(string? description)
        {
            EnsureDraft();
            Description = description;
            MarkAsUpdated();
        }
        public void SetPassingScore(int passingScorePercentage)
        {
            EnsureDraft();

            if (passingScorePercentage < 0 || passingScorePercentage > 100)
                throw new DomainException(ErrorCodes.Exam.InvalidPassingScore);

            PassingScorePercentage = passingScorePercentage;
            MarkAsUpdated();
        }

        public void SetDuration(int? durationMinutes)
        {
            EnsureDraft();

            if (durationMinutes.HasValue)
                DurationInMinutes = Guard.AgainstNegativeOrZero(durationMinutes.Value, nameof(durationMinutes));
            else
                DurationInMinutes = null;

            MarkAsUpdated();
        }
        public void AddQuestion(Question question)
        {
            EnsureDraft();

            question = Guard.AgainstNull(question, nameof(question));

            if (question.ExamId != Id)
                throw new DomainException(ErrorCodes.Exam.InvalidQuestionExamRelation);

            _questions.Add(question);

            MarkAsUpdated();
        }
        public void Publish()
        {
            EnsureDraft();

            if (!_questions.Any())
                throw new DomainException(ErrorCodes.Exam.MissingQuestions);

            IsPublished = true;
            MarkAsUpdated();
        }
        public void Unpublish(bool hasSubmissions)
        {
            if (!IsPublished)
                return;

            if (hasSubmissions)
                throw new DomainException(ErrorCodes.Exam.CannotUnpublishAfterAttempts);

            IsPublished = false;
            MarkAsUpdated();
        }
        public decimal TotalMarks() => _questions.Sum(q => q.QuestionMark);

        public decimal GetPassingScore() => TotalMarks() * PassingScorePercentage / 100m;

        public bool CanAttempt(int currentAttempts) => currentAttempts < MaxAttempts;
        private void EnsureDraft()
        {
            if (IsPublished)
                throw new DomainException(ErrorCodes.Exam.CannotModifyPublishedExam);
        }
        public void Delete() => MarkAsDeleted();
    }
}
