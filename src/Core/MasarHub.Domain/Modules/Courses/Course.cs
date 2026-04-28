using MasarHub.Domain.SharedKernel;
using MasarHub.Domain.SharedKernel.Base;
using MasarHub.Domain.SharedKernel.Exceptions;

namespace MasarHub.Domain.Modules.Courses
{
    public sealed class Course : SoftDeletableEntity
    {
        private readonly List<CoursePrerequisite> _prerequisites = [];
        private readonly List<CourseRequirement> _requirements = [];
        private readonly List<CourseLearningObjective> _learningObjectives = [];

        public string Title { get; private set; } = null!;
        public string Slug { get; private set; } = null!;
        public string Description { get; private set; } = null!;
        public decimal Price { get; private set; }
        public CourseLanguage Language { get; private set; }
        public CourseStatus Status { get; private set; }
        public CourseLevel Level { get; private set; }
        public string? ThumbnailUrl { get; private set; }
        public DateTimeOffset? PublishedAt { get; private set; }
        public Guid? ApprovedBy { get; private set; }
        public Guid? RejectedBy { get; private set; }
        public string? RejectionReason { get; private set; }


        // relations
        public Guid InstructorId { get; private set; }
        public Guid CategoryId { get; private set; }
        public IReadOnlyCollection<CoursePrerequisite> Prerequisites => _prerequisites;
        public IReadOnlyCollection<CourseRequirement> Requirements => _requirements;
        public IReadOnlyCollection<CourseLearningObjective> LearningObjectives => _learningObjectives;

        #region Constructors
        private Course() { }
        private Course(
            string title,
            string slug,
            string description,
            decimal price,
            CourseLanguage language,
            CourseLevel level,
            Guid instructorId,
            Guid categoryId,
            string? thumbnailUrl = null)
        {
            Title = Guard.AgainstNullOrWhiteSpace(title, nameof(title));
            Slug = Guard.AgainstNullOrWhiteSpace(slug, nameof(slug));
            Description = Guard.AgainstNullOrWhiteSpace(description, nameof(description));
            Price = Guard.AgainstNegative(price, nameof(price));
            InstructorId = Guard.AgainstEmptyGuid(instructorId, nameof(instructorId));
            CategoryId = Guard.AgainstEmptyGuid(categoryId, nameof(categoryId));
            Language = Guard.AgainstEnumOutOfRange(language, nameof(language));
            Level = Guard.AgainstEnumOutOfRange(level, nameof(level));
            ThumbnailUrl = thumbnailUrl;
            Status = CourseStatus.Draft;
        }

        #endregion

        #region Creation and Updates
        public static Course Create(
            string title,
            string slug,
            string description,
            decimal price,
            CourseLanguage language,
            CourseLevel level,
            Guid instructorId,
            Guid categoryId,
            string? thumbnailUrl = null) => new Course(title, slug, description, price, language, level, instructorId, categoryId, thumbnailUrl);

        public void UpdateTitle(string title)
        {
            Title = Guard.AgainstNullOrWhiteSpace(title, nameof(title));
            MarkAsUpdated();
        }
        public void UpdateDescription(string description)
        {
            Description = Guard.AgainstNullOrWhiteSpace(description, nameof(description));

            MarkAsUpdated();
        }
        public void UpdateThumbnailUrl(string? thumbnailUrl)
        {
            ThumbnailUrl = thumbnailUrl;
            MarkAsUpdated();
        }
        public void UpdatePrice(decimal price)
        {
            Price = Guard.AgainstNegative(price, nameof(price));

            MarkAsUpdated();
        }
        public void UpdateLanguage(CourseLanguage language)
        {
            Language = Guard.AgainstEnumOutOfRange(language, nameof(language));
            MarkAsUpdated();
        }
        public void UpdateLevel(CourseLevel level)
        {
            Level = Guard.AgainstEnumOutOfRange(level, nameof(level));
            MarkAsUpdated();
        }
        #endregion

        #region Course publication workflow

        public void SubmitForApproval()
        {
            if (Status != CourseStatus.Draft && Status != CourseStatus.Rejected)
                throw new DomainException(ErrorCodes.Course.InvalidStatusTransition);

            Status = CourseStatus.PendingApproval;
            MarkAsUpdated();
        }
        public void ApprovePublication(Guid adminId)
        {
            EnsurePendingApproval();

            Status = CourseStatus.Published;
            PublishedAt = DateTimeOffset.UtcNow;
            ApprovedBy = Guard.AgainstEmptyGuid(adminId, nameof(adminId));
            RejectedBy = null;
            RejectionReason = null;

            MarkAsUpdated();
        }
        public void RejectPublication(string reason, Guid adminId)
        {
            EnsurePendingApproval();

            Status = CourseStatus.Rejected;
            RejectionReason = Guard.AgainstNullOrWhiteSpace(reason, nameof(reason));
            PublishedAt = null;
            RejectedBy = Guard.AgainstEmptyGuid(adminId, nameof(adminId));

            MarkAsUpdated();
        }

        #endregion

        #region Prerequisites, Requirements and LearningObjective management
        public void AddPrerequisite(string value)
        {
            var item = CoursePrerequisite.Create(value);
            if (_prerequisites.Any(p => p.Value.Equals(item.Value, StringComparison.OrdinalIgnoreCase)))
                return;
            _prerequisites.Add(item);

            MarkAsUpdated();
        }
        public void AddRequirement(string value)
        {
            var item = CourseRequirement.Create(value);

            if (_requirements.Any(r => r.Value.Equals(item.Value, StringComparison.OrdinalIgnoreCase)))
                return;

            _requirements.Add(item);
            MarkAsUpdated();
        }
        public void AddLearningObjective(string value)
        {
            var item = CourseLearningObjective.Create(value);

            if (_learningObjectives.Any(r => r.Value.Equals(item.Value, StringComparison.OrdinalIgnoreCase)))
                return;

            _learningObjectives.Add(item);
            MarkAsUpdated();
        }

        public void RemoveRequirement(string value)
        {
            var item = _requirements
                .FirstOrDefault(r => r.Value.Equals(value, StringComparison.OrdinalIgnoreCase));

            if (item is null)
                return;

            _requirements.Remove(item);
            MarkAsUpdated();
        }
        public void RemovePrerequisite(string value)
        {
            var item = _prerequisites
                .FirstOrDefault(p => p.Value.Equals(value, StringComparison.OrdinalIgnoreCase));

            if (item is null) return;

            _prerequisites.Remove(item);
            MarkAsUpdated();
        }
        public void RemoveLearningObjective(string value)
        {
            var item = _learningObjectives
                .FirstOrDefault(p => p.Value.Equals(value, StringComparison.OrdinalIgnoreCase));

            if (item is null) return;

            _learningObjectives.Remove(item);
            MarkAsUpdated();
        }

        public void SetPrerequisites(IEnumerable<string> prerequisites)
        {
            _prerequisites.Clear();
            foreach (var prerequisite in prerequisites.Distinct(StringComparer.OrdinalIgnoreCase))
                _prerequisites.Add(CoursePrerequisite.Create(prerequisite));
            _prerequisites.AddRange(prerequisites.Distinct(StringComparer.OrdinalIgnoreCase).Select(CoursePrerequisite.Create));

            MarkAsUpdated();
        }
        public void SetRequirements(IEnumerable<string> requirements)
        {
            _requirements.Clear();
            foreach (var requirement in requirements.Distinct(StringComparer.OrdinalIgnoreCase))
                _requirements.Add(CourseRequirement.Create(requirement));

            MarkAsUpdated();
        }
        public void SetLearningObjective(IEnumerable<string> learningObjectives)
        {
            _learningObjectives.Clear();
            foreach (var learningObjective in learningObjectives.Distinct(StringComparer.OrdinalIgnoreCase))
                _learningObjectives.Add(CourseLearningObjective.Create(learningObjective));

            MarkAsUpdated();
        }
        #endregion

        #region Validation helpers
        private void EnsurePendingApproval()
        {
            if (Status != CourseStatus.PendingApproval)
                throw new DomainException(ErrorCodes.Course.NotPendingApproval);
        }

        #endregion
    }
}
