using MasarHub.Application.Abstractions.ExternalServices;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Abstractions.Services;
using MasarHub.Application.Common.DomainEvents;
using MasarHub.Domain.Modules.Courses.Events;
using MediatR;

namespace MasarHub.Application.Features.Courses.Commands.ApproveCourse
{
    public sealed class CourseApprovedEmailNotificationEventHandler
          : INotificationHandler<DomainEventNotification<CourseApprovedDomainEvent>>
    {
        private readonly IBackgroundJobService _backgroundJobService;
        private readonly IAppEmailService _appEmailService;
        private readonly ICourseQuery _courseQuery;
        public CourseApprovedEmailNotificationEventHandler(IBackgroundJobService backgroundJobService, IAppEmailService appEmailService, ICourseQuery courseQuery)
        {
            _backgroundJobService = backgroundJobService;
            _appEmailService = appEmailService;
            _courseQuery = courseQuery;
        }

        public async Task Handle(DomainEventNotification<CourseApprovedDomainEvent> notification, CancellationToken cancellationToken)
        {
            var domainEvent = notification.DomainEvent;
            (string fullName, string email) = await _courseQuery.GetInstructorInfoAsync(domainEvent.InstructorId, cancellationToken);

            _backgroundJobService.Enqueue(() =>
                _appEmailService.SendCourseApprovedEmailAsync(
                    fullName,
                    email,
                    domainEvent.CourseTitle,
                    $"/courses/{domainEvent.CourseId}"
                )
            );

        }
    }
}