using MasarHub.Application.Abstractions.ExternalServices;
using MasarHub.Application.Abstractions.Jobs;
using MasarHub.Application.Abstractions.Services;
using MasarHub.Application.Common.DomainEvents;
using MasarHub.Application.Common.Extensions;
using MasarHub.Domain.Modules.Courses.Events;
using MasarHub.Domain.Modules.Notifications;
using MediatR;

namespace MasarHub.Application.Features.Courses.Commands.RejectCourse
{
    public sealed class CourseRejectedNotificationEventHandler
           : INotificationHandler<DomainEventNotification<CourseRejectedDomainEvent>>
    {
        private readonly INotificationRealtimeService _notificationRealtimeService;
        private readonly IBackgroundJobService _backgroundJobService;

        public CourseRejectedNotificationEventHandler(INotificationRealtimeService notificationRealtimeService, IBackgroundJobService backgroundJobService)
        {
            _notificationRealtimeService = notificationRealtimeService;
            _backgroundJobService = backgroundJobService;
        }

        public async Task Handle(DomainEventNotification<CourseRejectedDomainEvent> notification, CancellationToken cancellationToken)
        {
            var domainEvent = notification.DomainEvent;

            var instructorNotificationResult = Notification.CreateForUser(
                userId: domainEvent.InstructorId,
                title: "Course Rejected",
                message: $"Your course '{domainEvent.CourseTitle}' has been rejected.",
                type: NotificationType.CourseRejected,
                priority: NotificationPriority.High,
                actionUrl: $"/courses/{domainEvent.CourseId}");

            if (instructorNotificationResult.IsSuccess)
            {
                await _notificationRealtimeService.SendToUserAsync(
                    domainEvent.InstructorId,
                    instructorNotificationResult.Value.ToRealtimeResponse(),
                    cancellationToken
                );

                _backgroundJobService.Enqueue<ICreateNotificationJob>(x =>
                    x.ExecuteAsync(instructorNotificationResult.Value.ToCreateRequest()));
            }
        }
    }
}
