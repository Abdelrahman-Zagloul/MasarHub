using MasarHub.Application.Abstractions.Jobs;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Abstractions.Persistence.Repositories;
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
        private readonly IRepository<CourseAnnouncement> _announcementRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AnnouncementJob(IAnnouncementQuery announcementQuery, INotificationRealtimeService notificationRealtimeService, IRepository<CourseAnnouncement> announcementRepository, IUnitOfWork unitOfWork)
        {
            _announcementQuery = announcementQuery;
            _notificationRealtimeService = notificationRealtimeService;
            _announcementRepository = announcementRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task NotifyAsync(Guid announcementId, Guid courseId, string title, string content, AnnouncementImportance importance)
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

        public async Task PublishAsync(Guid announcementId, Guid courseId)
        {
            var announcement = await _announcementRepository.GetByIdAsync(announcementId);
            if (announcement == null || announcement.CourseId != courseId || announcement.IsPublished)
                return;

            if (!announcement.ScheduledAt.HasValue || announcement.ScheduledAt.Value > DateTimeOffset.UtcNow)
                return;

            var result = announcement.Publish();
            if (result.IsFailure)
                return;

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
