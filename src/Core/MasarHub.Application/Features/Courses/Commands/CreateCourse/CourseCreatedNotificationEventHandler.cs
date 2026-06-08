using MasarHub.Application.Abstractions.ExternalServices;
using MasarHub.Application.Abstractions.Jobs;
using MasarHub.Application.Abstractions.Services;
using MasarHub.Application.Common.DomainEvents;
using MasarHub.Application.Common.Extensions;
using MasarHub.Domain.Modules.Courses.Events;
using MasarHub.Domain.Modules.Notifications;
using MediatR;

namespace MasarHub.Application.Features.Courses.Commands.CreateCourse
{
    public sealed class CourseCreatedNotificationEventHandler : INotificationHandler<DomainEventNotification<CourseCreatedDomainEvent>>
    {
        private readonly INotificationRealtimeService _notificationRealtimeService;
        private readonly IBackgroundJobService _backgroundJobService;

        public CourseCreatedNotificationEventHandler(INotificationRealtimeService notificationRealtimeService, IBackgroundJobService backgroundJobService)
        {
            _notificationRealtimeService = notificationRealtimeService;
            _backgroundJobService = backgroundJobService;
        }

        public async Task Handle(DomainEventNotification<CourseCreatedDomainEvent> notification, CancellationToken cancellationToken)
        {
            var actionUrl = $"/courses/{notification.DomainEvent.CourseId}";

            var notificationCreationResult = Notification.CreateForUser(
                userId: notification.DomainEvent.InstructorId,
                title: "Course Created",
                message: "Your course has been created successfully.",
                type: NotificationType.CourseCreated,
                priority: NotificationPriority.Normal,
                actionUrl: actionUrl);

            if (notificationCreationResult.IsFailure)
                return;

            await _notificationRealtimeService.SendToUserAsync(
                notification.DomainEvent.InstructorId,
                notificationCreationResult.Value.ToRealtimeResponse(),
                cancellationToken
            );

            _backgroundJobService.Enqueue<ICreateNotificationJob>(x => x.ExecuteAsync(notificationCreationResult.Value.ToCreateRequest()));
        }
    }
}


