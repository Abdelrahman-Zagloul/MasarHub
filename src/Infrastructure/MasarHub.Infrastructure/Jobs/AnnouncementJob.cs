using MasarHub.Application.Abstractions.Jobs;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Abstractions.Services;
using MasarHub.Application.Common.Extensions;
using MasarHub.Domain.Modules.Courses;
using MasarHub.Domain.Modules.Notifications;

namespace MasarHub.Infrastructure.Jobs
{
    public sealed class AnnouncementJob : IAnnouncementJob
    {
        private readonly IAnnouncementQuery _announcementQuery;
        private readonly INotificationRealtimeService _notificationRealtimeService;

        public AnnouncementJob(IAnnouncementQuery announcementQuery, INotificationRealtimeService notificationRealtimeService)
        {
            _announcementQuery = announcementQuery;
            _notificationRealtimeService = notificationRealtimeService;
        }

        public async Task ExecuteAsync(Guid announcementId, Guid courseId, string title, string content, AnnouncementImportance importance)
        {
            var actionUrl = $"/courses/{courseId}/announcements/{announcementId}";
            var notificationPriority = importance == AnnouncementImportance.High ? NotificationPriority.High : NotificationPriority.Normal;

            var userIds = await _announcementQuery.GetEnrolledUserIdsAsync(courseId);
            if (userIds.Count == 0)
                return;

            foreach (var userId in userIds)
            {
                var notificationResult = Notification.CreateForUser(
                    userId: userId,
                    title: $"New Announcement: {title}",
                    message: content,
                    type: NotificationType.CourseAnnouncementPublished,
                    priority: notificationPriority,
                    actionUrl: actionUrl,
                    resourceId: courseId);

                if (notificationResult.IsSuccess)
                    await _notificationRealtimeService.SendToUserAsync(userId, notificationResult.Value.ToRealtimeResponse());
            }
        }
    }
}
