using MasarHub.Application.Common.DI;
using MasarHub.Domain.Modules.Notifications;
using MasarHub.Domain.Modules.Profiles;

namespace MasarHub.Application.Abstractions.Jobs
{
    public interface ICreateNotificationJob : IScopedService
    {
        Task ExecuteAsync(CreateNotificationRequest request);
    }
    public sealed record CreateNotificationRequest
    (
          UserRole? TargetRole,
          Guid? UserId,
          string Title,
          string Message,
          NotificationType Type,
          NotificationPriority Priority,
          string? ActionUrl,
          Guid? ResourceId
    )
    {
        public static CreateNotificationRequest ForUser(Notification notification)
        {
            return new CreateNotificationRequest
            (
                null,
                notification.UserId,
                notification.Title,
                notification.Message,
                notification.Type,
                notification.Priority,
                notification.ActionUrl,
                notification.ResourceId
            );
        }
        public static CreateNotificationRequest ForRole(Notification notification)
        {
            return new
            (
                notification.TargetRole,
                null,
                notification.Title,
                notification.Message,
                notification.Type,
                notification.Priority,
                notification.ActionUrl,
                notification.ResourceId
            );
        }
        public Notification ToNotification()
        {
            if (UserId.HasValue)
            {
                return Notification.CreateForUser(
                    UserId.Value,
                    Title,
                    Message,
                    Type,
                    Priority,
                    ActionUrl,
                    ResourceId).Value;
            }

            return Notification.CreateForRole(
                TargetRole!.Value,
                Title,
                Message,
                Type,
                Priority,
                ActionUrl,
                ResourceId).Value;
        }
    }
}
