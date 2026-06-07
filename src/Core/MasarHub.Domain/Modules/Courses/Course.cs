using MasarHub.Domain.Common.Base;
using MasarHub.Domain.Common.Errors;
using MasarHub.Domain.Common.Guards;
using MasarHub.Domain.Common.Results;
using MasarHub.Domain.Modules.Courses.Events;

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
        public string? ThumbnailPublicId { get; private set; }
        public DateTimeOffset? PublishedAt { get; private set; }
        public Guid? ApprovedBy { get; private set; }
        public Guid? RejectedBy { get; private set; }
        public string? RejectionReason { get; private set; }
        public Guid InstructorId { get; private set; }
        public Guid CategoryId { get; private set; }
        public IReadOnlyCollection<CoursePrerequisite> Prerequisites => _prerequisites.AsReadOnly();
        public IReadOnlyCollection<CourseRequirement> Requirements => _requirements.AsReadOnly();
        public IReadOnlyCollection<CourseLearningObjective> LearningObjectives => _learningObjectives.AsReadOnly();

        #region Constractor
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
            string? thumbnailUrl)
        {
            Title = title;
            Slug = slug;
            Description = description;
            Price = price;
            Language = language;
            Level = level;
            InstructorId = instructorId;
            CategoryId = categoryId;
            ThumbnailPublicId = thumbnailUrl;
            Status = CourseStatus.Draft;
        }

        #endregion

        #region Course Management
        public static DomainResult<Course> Create(
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
            var error = GuardExtensions.FirstError(
                Guard.AgainstNullOrWhiteSpace(title, nameof(title)),
                Guard.AgainstNullOrWhiteSpace(slug, nameof(slug)),
                Guard.AgainstNullOrWhiteSpace(description, nameof(description)),
                Guard.AgainstNegative(price, nameof(price)),
                Guard.AgainstEnumOutOfRange(language, nameof(language)),
                Guard.AgainstEnumOutOfRange(level, nameof(level)),
                Guard.AgainstEmptyGuid(instructorId, nameof(instructorId)),
                Guard.AgainstEmptyGuid(categoryId, nameof(categoryId))
            );

            if (error is not null)
                return error;

            var course = new Course(title, slug, description, price, language, level, instructorId, categoryId, thumbnailUrl);

            course.RaiseDomainEvent(new CourseCreatedDomainEvent(course.Id, instructorId, title));
            return course;
        }

        public DomainResult UpdateTitle(string title)
        {
            var error = Guard.AgainstNullOrWhiteSpace(title, nameof(title));
            if (error != DomainError.None)
                return error;

            Title = title;
            MarkAsUpdated();
            return DomainResult.Success();
        }
        public DomainResult UpdateDescription(string description)
        {
            var error = Guard.AgainstNullOrWhiteSpace(description, nameof(description));
            if (error != DomainError.None)
                return error;

            Description = description;
            MarkAsUpdated();
            return DomainResult.Success();
        }
        public DomainResult UpdateThumbnailPublicId(string? thumbnailPublicId)
        {
            ThumbnailPublicId = thumbnailPublicId;
            MarkAsUpdated();
            return DomainResult.Success();
        }
        public DomainResult UpdatePrice(decimal price)
        {
            var error = Guard.AgainstNegative(price, nameof(price));
            if (error != DomainError.None)
                return error;

            Price = price;
            MarkAsUpdated();
            return DomainResult.Success();
        }
        public DomainResult UpdateLanguage(CourseLanguage language)
        {
            var error = Guard.AgainstEnumOutOfRange(language, nameof(language));
            if (error != DomainError.None)
                return error;

            Language = language;
            MarkAsUpdated();
            return DomainResult.Success();
        }
        public DomainResult UpdateLevel(CourseLevel level)
        {
            var error = Guard.AgainstEnumOutOfRange(level, nameof(level));
            if (error != DomainError.None)
                return error;

            Level = level;
            MarkAsUpdated();
            return DomainResult.Success();
        }
        public DomainResult UpdateCategory(Guid categoryId)
        {
            var error = Guard.AgainstEmptyGuid(categoryId, nameof(categoryId));
            if (error != DomainError.None)
                return error;

            CategoryId = categoryId;
            MarkAsUpdated();
            return DomainResult.Success();
        }
        public DomainResult Delete() => MarkAsDeleted();
        #endregion

        #region Course Submiting & Publication
        public DomainResult SubmitForApproval()
        {
            if (Status != CourseStatus.Draft && Status != CourseStatus.Rejected)
                return CourseErrors.InvalidStatusTransition;

            Status = CourseStatus.PendingApproval;
            RaiseDomainEvent(new CourseSubmittedForApprovalDomainEvent(Id, InstructorId));

            MarkAsUpdated();
            return DomainResult.Success();
        }
        public DomainResult ApprovePublication(Guid adminId)
        {
            var pendingResult = EnsurePendingApproval();
            if (pendingResult.IsFailure)
                return pendingResult;

            var error = Guard.AgainstEmptyGuid(adminId, nameof(adminId));
            if (error != DomainError.None)
                return error;

            Status = CourseStatus.Published;
            PublishedAt = DateTimeOffset.UtcNow;
            ApprovedBy = adminId;
            RejectedBy = null;
            RejectionReason = null;
            MarkAsUpdated();

            RaiseDomainEvent(new CourseApprovedDomainEvent(Id, InstructorId, Title));
            return DomainResult.Success();
        }
        public DomainResult RejectPublication(string reason, Guid adminId)
        {
            var error = GuardExtensions.FirstError(
                Guard.AgainstNullOrWhiteSpace(reason, nameof(reason)),
                Guard.AgainstEmptyGuid(adminId, nameof(adminId))
            );
            if (error != null)
                return error;

            var pendingResult = EnsurePendingApproval();
            if (pendingResult.IsFailure)
                return pendingResult;

            Status = CourseStatus.Rejected;
            RejectionReason = reason;
            PublishedAt = null;
            RejectedBy = adminId;
            MarkAsUpdated();

            return DomainResult.Success();
        }

        #endregion

        #region CoursePrerequisite & CourseRequirement & CourseLearningObjective Management

        public DomainResult SetPrerequisites(IEnumerable<string> prerequisites)
        {
            _prerequisites.Clear();
            foreach (var prerequisite in prerequisites.Distinct(StringComparer.OrdinalIgnoreCase))
            {
                var result = CoursePrerequisite.Create(prerequisite);
                if (result.IsFailure)
                    return result.Error;

                _prerequisites.Add(result.Value);
            }

            MarkAsUpdated();
            return DomainResult.Success();
        }

        public DomainResult SetRequirements(IEnumerable<string> requirements)
        {
            _requirements.Clear();
            foreach (var requirement in requirements.Distinct(StringComparer.OrdinalIgnoreCase))
            {
                var result = CourseRequirement.Create(requirement);
                if (result.IsFailure)
                    return result.Error;

                _requirements.Add(result.Value);
            }

            MarkAsUpdated();
            return DomainResult.Success();
        }

        public DomainResult SetLearningObjectives(IEnumerable<string> learningObjectives)
        {
            _learningObjectives.Clear();
            foreach (var learningObjective in learningObjectives.Distinct(StringComparer.OrdinalIgnoreCase))
            {
                var result = CourseLearningObjective.Create(learningObjective);
                if (result.IsFailure)
                    return result.Error;

                _learningObjectives.Add(result.Value);
            }

            MarkAsUpdated();
            return DomainResult.Success();
        }

        #endregion

        private DomainResult EnsurePendingApproval()
        {
            return Status == CourseStatus.PendingApproval
                ? DomainResult.Success()
                : CourseErrors.NotPendingApproval;
        }
    }
}
