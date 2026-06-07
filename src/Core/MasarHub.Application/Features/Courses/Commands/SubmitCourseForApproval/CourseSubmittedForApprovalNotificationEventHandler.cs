using MasarHub.Application.Abstractions.ExternalServices;
using MasarHub.Application.Abstractions.Jobs;
using MasarHub.Application.Abstractions.Services;
using MasarHub.Application.Common.DomainEvents;
using MasarHub.Application.Common.Extensions;
using MasarHub.Domain.Modules.Courses.Events;
using MasarHub.Domain.Modules.Notifications;
using MasarHub.Domain.Modules.Profiles;
using MediatR;

namespace MasarHub.Application.Features.Courses.Commands.SubmitCourseForApproval
{
    public sealed class CourseSubmittedForApprovalNotificationEventHandler
         : INotificationHandler<DomainEventNotification<CourseSubmittedForApprovalDomainEvent>>
    {
        private readonly INotificationRealtimeService _notificationRealtimeService;
        private readonly IBackgroundJobService _backgroundJobService;

        public CourseSubmittedForApprovalNotificationEventHandler(INotificationRealtimeService notificationRealtimeService, IBackgroundJobService backgroundJobService)
        {
            _notificationRealtimeService = notificationRealtimeService;
            _backgroundJobService = backgroundJobService;
        }

        public async Task Handle(DomainEventNotification<CourseSubmittedForApprovalDomainEvent> notification, CancellationToken cancellationToken)
        {
            var actionUrl = $"/courses/{notification.DomainEvent.CourseId}";

            // Notify admins about the new course submission
            var adminNotificationResult = Notification.CreateForRole(
                targetRole: UserRole.Admin,
                title: "New Course Submitted",
                message: "New course has been submitted for approval.",
                type: NotificationType.CourseSubmittedForApproval,
                priority: NotificationPriority.High,
                actionUrl: actionUrl);

            if (adminNotificationResult.IsSuccess)
            {
                await _notificationRealtimeService.SendToAdminsAsync(
                    adminNotificationResult.Value.ToRealtimeResponse(),
                    cancellationToken
                );

                _backgroundJobService.Enqueue<ICreateNotificationJob>(x =>
                    x.ExecuteAsync(adminNotificationResult.Value.ToCreateRequest()));
            }

            // Notify the instructor about their course submission
            var instructorNotificationResult = Notification.CreateForUser(
                userId: notification.DomainEvent.InstructorId,
                title: "Course Submitted for Approval",
                message: "Your course has been submitted for approval.",
                type: NotificationType.CourseSubmittedForApproval,
                priority: NotificationPriority.High,
                actionUrl: actionUrl);

            if (instructorNotificationResult.IsSuccess)
            {
                await _notificationRealtimeService.SendToUserAsync(
                    notification.DomainEvent.InstructorId,
                    instructorNotificationResult.Value.ToRealtimeResponse(),
                    cancellationToken
                );

                _backgroundJobService.Enqueue<ICreateNotificationJob>(x =>
                    x.ExecuteAsync(instructorNotificationResult.Value.ToCreateRequest()));
            }
        }
    }
}
