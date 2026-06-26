using MasarHub.Application.Abstractions.ExternalServices;
using MasarHub.Application.Abstractions.Jobs;
using MasarHub.Application.Abstractions.Services;
using MasarHub.Application.Common.DomainEvents;
using MasarHub.Application.Common.Extensions;
using MasarHub.Domain.Modules.Courses.Events;
using MasarHub.Domain.Modules.Notifications;
using MediatR;

namespace MasarHub.Application.Features.Payments.Events
{
    public sealed class CourseEnrollmentCreatedNotificationEventHandler
        : INotificationHandler<DomainEventNotification<CourseEnrollmentCreatedDomainEvent>>
    {
        private readonly INotificationRealtimeService _notificationRealtimeService;
        private readonly IBackgroundJobService _backgroundJobService;

        public CourseEnrollmentCreatedNotificationEventHandler(INotificationRealtimeService notificationRealtimeService, IBackgroundJobService backgroundJobService)
        {
            _notificationRealtimeService = notificationRealtimeService;
            _backgroundJobService = backgroundJobService;
        }

        public async Task Handle(DomainEventNotification<CourseEnrollmentCreatedDomainEvent> notification, CancellationToken cancellationToken)
        {
            var domainEvent = notification.DomainEvent;
            var actionUrl = $"/courses/{domainEvent.CourseId}";

            var notificationResult = Notification.CreateForUser(
                userId: domainEvent.UserId,
                title: "Course Enrollment Created",
                message: $"You are now enrolled in {domainEvent.CourseTitle}.",
                type: NotificationType.CourseEnrollmentCreated,
                priority: NotificationPriority.High,
                actionUrl: actionUrl);

            if (notificationResult.IsFailure)
                return;

            await _notificationRealtimeService.SendToUserAsync(domainEvent.UserId, notificationResult.Value.ToRealtimeResponse(), cancellationToken);
            _backgroundJobService.Enqueue<ICreateNotificationJob>(x => x.ExecuteAsync(notificationResult.Value.ToCreateRequest()));
        }
    }
}
