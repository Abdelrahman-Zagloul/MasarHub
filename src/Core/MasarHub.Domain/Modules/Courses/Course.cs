using MasarHub.Domain.Common.Base;
using MasarHub.Domain.Common.Guards;
using MasarHub.Domain.Common.Results;

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
            ThumbnailUrl = thumbnailUrl;
            Status = CourseStatus.Draft;
        }

        #endregion

        #region Course Management
        public static Result<Course> Create(
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

            return new Course(title, slug, description, price, language, level, instructorId, categoryId, thumbnailUrl);
        }

        public Result UpdateTitle(string title)
        {
            var error = Guard.AgainstNullOrWhiteSpace(title, nameof(title));
            if (error is not null)
                return error;

            Title = title;
            MarkAsUpdated();
            return Result.Success();
        }

        public Result UpdateDescription(string description)
        {
            var error = Guard.AgainstNullOrWhiteSpace(description, nameof(description));
            if (error is not null)
                return error;

            Description = description;
            MarkAsUpdated();
            return Result.Success();
        }

        public Result UpdateThumbnailUrl(string? thumbnailUrl)
        {
            ThumbnailUrl = thumbnailUrl;
            MarkAsUpdated();
            return Result.Success();
        }

        public Result UpdatePrice(decimal price)
        {
            var error = Guard.AgainstNegative(price, nameof(price));
            if (error is not null)
                return error;

            Price = price;
            MarkAsUpdated();
            return Result.Success();
        }

        public Result UpdateLanguage(CourseLanguage language)
        {
            var error = Guard.AgainstEnumOutOfRange(language, nameof(language));
            if (error is not null)
                return error;

            Language = language;
            MarkAsUpdated();
            return Result.Success();
        }

        public Result UpdateLevel(CourseLevel level)
        {
            var error = Guard.AgainstEnumOutOfRange(level, nameof(level));
            if (error is not null)
                return error;

            Level = level;
            MarkAsUpdated();
            return Result.Success();
        }
        public Result Delete() => MarkAsDeleted();
        #endregion

        #region Course Submiting & Publication

        public Result SubmitForApproval()
        {
            if (Status != CourseStatus.Draft && Status != CourseStatus.Rejected)
                return CourseErrors.InvalidStatusTransition;

            Status = CourseStatus.PendingApproval;
            MarkAsUpdated();
            return Result.Success();
        }

        public Result ApprovePublication(Guid adminId)
        {
            var pendingResult = EnsurePendingApproval();
            if (pendingResult.IsFailure)
                return pendingResult;

            var error = Guard.AgainstEmptyGuid(adminId, nameof(adminId));
            if (error is not null)
                return error;

            Status = CourseStatus.Published;
            PublishedAt = DateTimeOffset.UtcNow;
            ApprovedBy = adminId;
            RejectedBy = null;
            RejectionReason = null;
            MarkAsUpdated();

            return Result.Success();
        }

        public Result RejectPublication(string reason, Guid adminId)
        {
            var error = GuardExtensions.FirstError(
                Guard.AgainstNullOrWhiteSpace(reason, nameof(reason)),
                Guard.AgainstEmptyGuid(adminId, nameof(adminId))
            );
            if (error is not null)
                return error;

            var pendingResult = EnsurePendingApproval();
            if (pendingResult.IsFailure)
                return pendingResult;

            Status = CourseStatus.Rejected;
            RejectionReason = reason;
            PublishedAt = null;
            RejectedBy = adminId;
            MarkAsUpdated();

            return Result.Success();
        }

        #endregion

        #region CoursePrerequisite & CourseRequirement & CourseLearningObjective Management

        public Result AddPrerequisite(string value)
        {
            var result = CoursePrerequisite.Create(value);
            if (result.IsFailure)
                return result.Error;

            var item = result.Value!;
            if (_prerequisites.Any(p => p.Value.Equals(item.Value, StringComparison.OrdinalIgnoreCase)))
                return Result.Success();

            _prerequisites.Add(item);
            MarkAsUpdated();
            return Result.Success();
        }

        public Result AddRequirement(string value)
        {
            var result = CourseRequirement.Create(value);
            if (result.IsFailure)
                return result.Error;

            var item = result.Value!;
            if (_requirements.Any(r => r.Value.Equals(item.Value, StringComparison.OrdinalIgnoreCase)))
                return Result.Success();

            _requirements.Add(item);
            MarkAsUpdated();
            return Result.Success();
        }

        public Result AddLearningObjective(string value)
        {
            var result = CourseLearningObjective.Create(value);
            if (result.IsFailure)
                return result.Error;

            var item = result.Value!;
            if (_learningObjectives.Any(l => l.Value.Equals(item.Value, StringComparison.OrdinalIgnoreCase)))
                return Result.Success();

            _learningObjectives.Add(item);
            MarkAsUpdated();
            return Result.Success();
        }

        public Result RemoveRequirement(string value)
        {
            var item = _requirements.FirstOrDefault(r => r.Value.Equals(value, StringComparison.OrdinalIgnoreCase));
            if (item is null)
                return Result.Success();

            _requirements.Remove(item);
            MarkAsUpdated();
            return Result.Success();
        }

        public Result RemovePrerequisite(string value)
        {
            var item = _prerequisites.FirstOrDefault(p => p.Value.Equals(value, StringComparison.OrdinalIgnoreCase));
            if (item is null)
                return Result.Success();

            _prerequisites.Remove(item);
            MarkAsUpdated();
            return Result.Success();
        }

        public Result RemoveLearningObjective(string value)
        {
            var item = _learningObjectives.FirstOrDefault(l => l.Value.Equals(value, StringComparison.OrdinalIgnoreCase));
            if (item is null)
                return Result.Success();

            _learningObjectives.Remove(item);
            MarkAsUpdated();
            return Result.Success();
        }

        public Result SetPrerequisites(IEnumerable<string> prerequisites)
        {
            _prerequisites.Clear();
            foreach (var prerequisite in prerequisites.Distinct(StringComparer.OrdinalIgnoreCase))
            {
                var result = CoursePrerequisite.Create(prerequisite);
                if (result.IsFailure)
                    return result.Error;

                _prerequisites.Add(result.Value!);
            }

            MarkAsUpdated();
            return Result.Success();
        }

        public Result SetRequirements(IEnumerable<string> requirements)
        {
            _requirements.Clear();
            foreach (var requirement in requirements.Distinct(StringComparer.OrdinalIgnoreCase))
            {
                var result = CourseRequirement.Create(requirement);
                if (result.IsFailure)
                    return result.Error;

                _requirements.Add(result.Value!);
            }

            MarkAsUpdated();
            return Result.Success();
        }

        public Result SetLearningObjective(IEnumerable<string> learningObjectives)
        {
            _learningObjectives.Clear();
            foreach (var learningObjective in learningObjectives.Distinct(StringComparer.OrdinalIgnoreCase))
            {
                var result = CourseLearningObjective.Create(learningObjective);
                if (result.IsFailure)
                    return result.Error;

                _learningObjectives.Add(result.Value!);
            }

            MarkAsUpdated();
            return Result.Success();
        }

        #endregion

        private Result EnsurePendingApproval()
        {
            return Status == CourseStatus.PendingApproval
                ? Result.Success()
                : CourseErrors.NotPendingApproval;
        }
    }
}
