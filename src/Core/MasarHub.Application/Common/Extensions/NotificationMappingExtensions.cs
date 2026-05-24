using MasarHub.Application.Common.Models.Notifications;
using MasarHub.Domain.Modules.Notifications;

namespace MasarHub.Application.Common.Extensions
{
    public static class NotificationMappingExtensions
    {
        public static NotificationRealtimeResponse ToRealtimeResponse(this Notification notification)
        {
            return new NotificationRealtimeResponse(
                notification.Id,
                notification.Title,
                notification.Message,
                notification.Type,
                notification.Priority,
                notification.ActionUrl,
                notification.CreatedAt);
        }

        public static CreateNotificationRequest ToCreateRequest(this Notification notification)
        {
            return notification.UserId.HasValue
                ? CreateNotificationRequest.ForUser(notification)
                : CreateNotificationRequest.ForRole(notification);
        }
    }
}
