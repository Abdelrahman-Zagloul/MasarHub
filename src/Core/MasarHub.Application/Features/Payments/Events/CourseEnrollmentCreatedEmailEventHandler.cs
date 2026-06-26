using MasarHub.Application.Abstractions.ExternalServices;
using MasarHub.Application.Abstractions.Jobs;
using MasarHub.Application.Common.DomainEvents;
using MasarHub.Domain.Modules.Courses.Events;
using MediatR;

namespace MasarHub.Application.Features.Payments.Events
{
    public sealed class CourseEnrollmentCreatedEmailEventHandler
        : INotificationHandler<DomainEventNotification<CourseEnrollmentCreatedDomainEvent>>
    {
        private readonly IBackgroundJobService _backgroundJobService;

        public CourseEnrollmentCreatedEmailEventHandler(IBackgroundJobService backgroundJobService)
        {
            _backgroundJobService = backgroundJobService;
        }

        public Task Handle(DomainEventNotification<CourseEnrollmentCreatedDomainEvent> notification, CancellationToken cancellationToken)
        {
            _backgroundJobService.Enqueue<IEmailJob>(x =>
                x.SendCourseEnrollmentCreatedEmailAsync(
                    notification.DomainEvent.UserId,
                    notification.DomainEvent.CourseTitle,
                    notification.DomainEvent.PaidAmount,
                    notification.DomainEvent.CourseId
                )
            );

            return Task.CompletedTask;
        }
    }
}
