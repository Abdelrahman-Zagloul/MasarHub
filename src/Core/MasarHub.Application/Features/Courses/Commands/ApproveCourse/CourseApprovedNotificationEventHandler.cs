using MasarHub.Application.Abstractions.ExternalServices;
using MasarHub.Application.Abstractions.Jobs;
using MasarHub.Application.Abstractions.Services;
using MasarHub.Application.Common.DomainEvents;
using MasarHub.Application.Common.Extensions;
using MasarHub.Domain.Modules.Courses.Events;
using MasarHub.Domain.Modules.Notifications;
using MediatR;

namespace MasarHub.Application.Features.Courses.Commands.ApproveCourse
{
    public sealed class CourseApprovedNotificationEventHandler
         : INotificationHandler<DomainEventNotification<CourseApprovedDomainEvent>>
    {
        private readonly INotificationRealtimeService _notificationRealtimeService;
        private readonly IBackgroundJobService _backgroundJobService;

        public CourseApprovedNotificationEventHandler(INotificationRealtimeService notificationRealtimeService, IBackgroundJobService backgroundJobService)
        {
            _notificationRealtimeService = notificationRealtimeService;
            _backgroundJobService = backgroundJobService;
        }

        public async Task Handle(DomainEventNotification<CourseApprovedDomainEvent> notification, CancellationToken cancellationToken)
        {
            var domainEvent = notification.DomainEvent;
            var actionUrl = $"/courses/{domainEvent.CourseId}";

            var instructorNotificationResult = Notification.CreateForUser(
                userId: domainEvent.InstructorId,
                title: "Course Approved & Published",
                message: "Congratulations! Your course has been approved and is now live for students.",
                type: NotificationType.CourseApproved,
                priority: NotificationPriority.High,
                actionUrl: actionUrl);

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
