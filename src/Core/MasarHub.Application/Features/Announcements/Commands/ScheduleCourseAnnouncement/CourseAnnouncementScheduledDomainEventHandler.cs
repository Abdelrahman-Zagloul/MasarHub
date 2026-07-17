using MasarHub.Application.Abstractions.ExternalServices;
using MasarHub.Application.Abstractions.Jobs;
using MasarHub.Application.Common.DomainEvents;
using MasarHub.Domain.Modules.Courses.Events;
using MediatR;

namespace MasarHub.Application.Features.Announcements.Commands.ScheduleCourseAnnouncement
{
    public sealed class CourseAnnouncementScheduledDomainEventHandler
        : INotificationHandler<DomainEventNotification<CourseAnnouncementScheduledDomainEvent>>
    {
        private readonly IBackgroundJobService _backgroundJobService;

        public CourseAnnouncementScheduledDomainEventHandler(IBackgroundJobService backgroundJobService)
        {
            _backgroundJobService = backgroundJobService;
        }

        public async Task Handle(DomainEventNotification<CourseAnnouncementScheduledDomainEvent> notification, CancellationToken cancellationToken)
        {
            var domainEvent = notification.DomainEvent;

            _backgroundJobService.Schedule<IAnnouncementJob>(
                x => x.PublishAsync(domainEvent.AnnouncementId, domainEvent.CourseId),
                domainEvent.ScheduledAt);
        }
    }
}
