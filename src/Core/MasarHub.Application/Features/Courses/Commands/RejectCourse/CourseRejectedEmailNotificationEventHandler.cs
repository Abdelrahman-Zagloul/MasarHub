using MasarHub.Application.Abstractions.ExternalServices;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Abstractions.Services;
using MasarHub.Application.Common.DomainEvents;
using MasarHub.Domain.Modules.Courses.Events;
using MediatR;

namespace MasarHub.Application.Features.Courses.Commands.RejectCourse
{
    public sealed class CourseRejectedEmailNotificationEventHandler
          : INotificationHandler<DomainEventNotification<CourseRejectedDomainEvent>>
    {
        private readonly IBackgroundJobService _backgroundJobService;
        private readonly ICourseQuery _courseQuery;

        public CourseRejectedEmailNotificationEventHandler(IBackgroundJobService backgroundJobService, ICourseQuery courseQuery)
        {
            _backgroundJobService = backgroundJobService;
            _courseQuery = courseQuery;
        }

        public async Task Handle(DomainEventNotification<CourseRejectedDomainEvent> notification, CancellationToken cancellationToken)
        {
            var domainEvent = notification.DomainEvent;
            var instructorInfo = await _courseQuery.GetInstructorInfoAsync(domainEvent.InstructorId, cancellationToken);
            if (instructorInfo == null)
                return;

            _backgroundJobService.Enqueue<IAppEmailService>(x =>
                x.SendCourseRejectedEmailAsync(
                    instructorInfo.FullName,
                    instructorInfo.Email,
                    domainEvent.CourseTitle,
                    domainEvent.RejectionReason,
                    $"/courses/{domainEvent.CourseId}"
                )
            );
        }
    }
}