using MasarHub.Domain.Modules.Notifications;

namespace MasarHub.Application.Common.Models.Notifications
{
    public sealed record NotificationRealtimeResponse
    (
         Guid Id,
         string Title,
         string Message,
         NotificationType Type,
         NotificationPriority Priority,
         string? ActionUrl,
         DateTimeOffset CreatedAt
    )
    {
        public static NotificationRealtimeResponse ToRealtimeResponse(Notification notification)
        {
            return new NotificationRealtimeResponse
            (
                notification.Id,
                notification.Title,
                notification.Message,
                notification.Type,
                notification.Priority,
                notification.ActionUrl,
                notification.CreatedAt
            );
        }
    }
}
