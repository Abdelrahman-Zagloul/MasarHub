using MasarHub.Domain.Common.Base;
using MasarHub.Domain.Common.Guards;
using MasarHub.Domain.Common.Results;
using MasarHub.Domain.Modules.Profiles;

namespace MasarHub.Domain.Modules.Notifications
{
    public sealed class Notification : SoftDeletableEntity
    {
        public UserRole TargetRole { get; private set; }
        public string Title { get; private set; } = null!;
        public string Message { get; private set; } = null!;
        public NotificationType Type { get; private set; }
        public NotificationPriority Priority { get; private set; }
        public bool IsRead { get; private set; }
        public DateTimeOffset? ReadAt { get; private set; }
        public string? ActionUrl { get; private set; }
        public Guid? ResourceId { get; private set; }
        private Notification() { }

        private Notification(
            UserRole targetRole,
            string title,
            string message,
            NotificationType type,
            NotificationPriority priority,
            string? actionUrl,
            Guid? resourceId)
        {
            TargetRole = targetRole;
            Title = title;
            Message = message;
            Type = type;
            Priority = priority;
            ActionUrl = actionUrl;
            ResourceId = resourceId;
        }

        public static Result<Notification> Create(
            UserRole targetRole,
            string title,
            string message,
            NotificationType type,
            NotificationPriority priority = NotificationPriority.Normal,
            string? actionUrl = null,
            Guid? resourceId = null)
        {
            var error = GuardExtensions.FirstError(
                Guard.AgainstEnumOutOfRange(targetRole, nameof(targetRole)),
                Guard.AgainstNullOrWhiteSpace(title, nameof(title)),
                Guard.AgainstNullOrWhiteSpace(message, nameof(message)),
                Guard.AgainstEnumOutOfRange(type, nameof(type)),
                Guard.AgainstEnumOutOfRange(priority, nameof(priority))
            );

            if (error is not null)
                return error;

            if (actionUrl is not null && string.IsNullOrWhiteSpace(actionUrl))
                return Guard.AgainstNullOrWhiteSpace(actionUrl, nameof(actionUrl))!;

            if (resourceId.HasValue && resourceId == Guid.Empty)
                return Guard.AgainstEmptyGuid(resourceId.Value, nameof(resourceId))!;

            return new Notification(
                targetRole,
                title.Trim(),
                message.Trim(),
                type,
                priority,
                actionUrl?.Trim(),
                resourceId);
        }

        public void MarkAsRead()
        {
            if (IsRead)
                return;

            IsRead = true;
            ReadAt = DateTimeOffset.UtcNow;

            MarkAsUpdated();
        }

        public void MarkAsUnread()
        {
            if (!IsRead)
                return;

            IsRead = false;
            ReadAt = null;

            MarkAsUpdated();
        }
    }
}