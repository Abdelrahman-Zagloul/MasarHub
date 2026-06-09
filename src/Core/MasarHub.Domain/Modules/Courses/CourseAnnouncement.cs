using MasarHub.Domain.Common.Base;
using MasarHub.Domain.Common.Errors;
using MasarHub.Domain.Common.Guards;
using MasarHub.Domain.Common.Results;

namespace MasarHub.Domain.Modules.Courses
{
    public sealed class CourseAnnouncement : SoftDeletableEntity
    {
        public Guid CourseId { get; private set; }
        public Guid InstructorId { get; private set; }
        public string Title { get; private set; } = null!;
        public string Content { get; private set; } = null!;
        public bool IsPublished { get; private set; }
        public DateTimeOffset? PublishedAt { get; private set; }
        public DateTimeOffset? ScheduledAt { get; private set; }
        public DateTimeOffset? ExpiresAt { get; private set; }
        public AnnouncementImportance Importance { get; private set; }
        public bool IsPinned { get; private set; }

        private CourseAnnouncement() { }

        private CourseAnnouncement(
            Guid courseId,
            Guid instructorId,
            string title,
            string content,
            AnnouncementImportance importance)
        {
            CourseId = courseId;
            InstructorId = instructorId;
            Title = title;
            Content = content;
            Importance = importance;
            IsPinned = false;
            IsPublished = false;
        }

        public static DomainResult<CourseAnnouncement> Create(
            Guid courseId,
            Guid instructorId,
            string title,
            string content,
            AnnouncementImportance importance = AnnouncementImportance.Normal)
        {
            var error = GuardExtensions.FirstError(
                Guard.AgainstEmptyGuid(courseId, nameof(courseId)),
                Guard.AgainstEmptyGuid(instructorId, nameof(instructorId)),
                Guard.AgainstNullOrWhiteSpace(title, nameof(title)),
                Guard.AgainstNullOrWhiteSpace(content, nameof(content)),
                Guard.AgainstEnumOutOfRange(importance, nameof(importance))
            );

            if (error is not null)
                return error;

            return new CourseAnnouncement(courseId, instructorId, title, content, importance);
        }

        public DomainResult UpdateTitle(string title)
        {
            var editable = EnsureEditable();
            if (editable.IsFailure)
                return editable;

            var error = Guard.AgainstNullOrWhiteSpace(title, nameof(title));
            if (error != DomainError.None)
                return error;

            Title = title;
            MarkAsUpdated();
            return DomainResult.Success();
        }

        public DomainResult UpdateContent(string content)
        {
            var editable = EnsureEditable();
            if (editable.IsFailure)
                return editable;

            var error = Guard.AgainstNullOrWhiteSpace(content, nameof(content));
            if (error != DomainError.None)
                return error;

            Content = content;
            MarkAsUpdated();
            return DomainResult.Success();
        }

        public DomainResult Publish()
        {
            if (IsPublished)
                return CourseAnnouncementErrors.AlreadyPublished;

            if (ExpiresAt.HasValue && ExpiresAt <= DateTimeOffset.UtcNow)
                return CourseAnnouncementErrors.InvalidExpirationTime;

            IsPublished = true;
            PublishedAt = DateTimeOffset.UtcNow;
            MarkAsUpdated();
            return DomainResult.Success();
        }

        public DomainResult Schedule(DateTimeOffset scheduledAt)
        {
            if (IsPublished)
                return CourseAnnouncementErrors.AlreadyPublished;

            if (scheduledAt <= DateTimeOffset.UtcNow)
                return CourseAnnouncementErrors.InvalidScheduleTime;

            ScheduledAt = scheduledAt;
            MarkAsUpdated();
            return DomainResult.Success();
        }

        public DomainResult SetExpiration(DateTimeOffset expiresAt)
        {
            if (expiresAt <= DateTimeOffset.UtcNow)
                return CourseAnnouncementErrors.InvalidExpirationTime;

            ExpiresAt = expiresAt;
            MarkAsUpdated();
            return DomainResult.Success();
        }

        public DomainResult SetImportance(AnnouncementImportance importance)
        {
            var error = Guard.AgainstEnumOutOfRange(importance, nameof(importance));
            if (error != DomainError.None)
                return error;

            Importance = importance;
            MarkAsUpdated();
            return DomainResult.Success();
        }

        public DomainResult Pin()
        {
            IsPinned = true;
            MarkAsUpdated();
            return DomainResult.Success();
        }

        public DomainResult Unpin()
        {
            IsPinned = false;
            MarkAsUpdated();
            return DomainResult.Success();
        }

        public DomainResult Delete() => MarkAsDeleted();

        private DomainResult EnsureEditable() => IsPublished ? CourseAnnouncementErrors.CannotEditAfterPublish : DomainResult.Success();
    }
}
