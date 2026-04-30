using MasarHub.Domain.SharedKernel;
using MasarHub.Domain.SharedKernel.Base;
using MasarHub.Domain.SharedKernel.Exceptions;

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
            CourseId = Guard.AgainstEmptyGuid(courseId, nameof(courseId));
            InstructorId = Guard.AgainstEmptyGuid(instructorId, nameof(instructorId));
            Title = Guard.AgainstNullOrWhiteSpace(title, nameof(title));
            Content = Guard.AgainstNullOrWhiteSpace(content, nameof(content));
            Importance = Guard.AgainstEnumOutOfRange(importance, nameof(importance));

            IsPinned = false;
            IsPublished = false;
        }

        public static CourseAnnouncement Create(
            Guid courseId,
            Guid instructorId,
            string title,
            string content,
            AnnouncementImportance importance = AnnouncementImportance.Normal)
            => new(courseId, instructorId, title, content, importance);

        private void EnsureEditable()
        {
            if (IsPublished)
                throw new DomainException(ErrorCodes.CourseAnnouncement.CannotEditAfterPublish);
        }

        public void UpdateTitle(string title)
        {
            EnsureEditable();
            Title = Guard.AgainstNullOrWhiteSpace(title, nameof(title));
            MarkAsUpdated();
        }

        public void UpdateContent(string content)
        {
            EnsureEditable();
            Content = Guard.AgainstNullOrWhiteSpace(content, nameof(content));
            MarkAsUpdated();
        }

        public void Publish()
        {
            if (IsPublished)
                throw new DomainException(ErrorCodes.CourseAnnouncement.AlreadyPublished);

            if (ExpiresAt.HasValue && ExpiresAt <= DateTimeOffset.UtcNow)
                throw new DomainException(ErrorCodes.CourseAnnouncement.InvalidExpirationTime);

            IsPublished = true;
            PublishedAt = DateTimeOffset.UtcNow;
            MarkAsUpdated();
        }

        public void Schedule(DateTimeOffset scheduledAt)
        {
            if (IsPublished)
                throw new DomainException(ErrorCodes.CourseAnnouncement.AlreadyPublished);

            if (scheduledAt <= DateTimeOffset.UtcNow)
                throw new DomainException(ErrorCodes.CourseAnnouncement.InvalidScheduleTime);

            ScheduledAt = scheduledAt;
            MarkAsUpdated();
        }

        public void SetExpiration(DateTimeOffset expiresAt)
        {
            if (expiresAt <= DateTimeOffset.UtcNow)
                throw new DomainException(ErrorCodes.CourseAnnouncement.InvalidExpirationTime);

            ExpiresAt = expiresAt;
            MarkAsUpdated();
        }

        public void SetImportance(AnnouncementImportance importance)
        {
            Importance = Guard.AgainstEnumOutOfRange(importance, nameof(importance));
            MarkAsUpdated();
        }

        public void Pin()
        {
            IsPinned = true;
            MarkAsUpdated();
        }

        public void Unpin()
        {
            IsPinned = false;
            MarkAsUpdated();
        }
    }
}