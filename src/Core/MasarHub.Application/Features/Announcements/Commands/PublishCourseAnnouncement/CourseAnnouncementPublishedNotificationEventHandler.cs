using MasarHub.Application.Abstractions.ExternalServices;
using MasarHub.Application.Abstractions.Jobs;
using MasarHub.Application.Common.DomainEvents;
using MasarHub.Domain.Modules.Courses.Events;
using MediatR;

namespace MasarHub.Application.Features.Announcements.Commands.PublishCourseAnnouncement
{
    public sealed class CourseAnnouncementPublishedNotificationEventHandler
        : INotificationHandler<DomainEventNotification<CourseAnnouncementPublishedDomainEvent>>
    {
        private readonly IBackgroundJobService _backgroundJobService;

        public CourseAnnouncementPublishedNotificationEventHandler(IBackgroundJobService backgroundJobService)
        {
            _backgroundJobService = backgroundJobService;
        }

        public async Task Handle(DomainEventNotification<CourseAnnouncementPublishedDomainEvent> notification, CancellationToken cancellationToken)
        {
            var domainEvent = notification.DomainEvent;

            _backgroundJobService.Enqueue<IAnnouncementJob>(x =>
                x.ExecuteAsync(
                    domainEvent.AnnouncementId,
                    domainEvent.CourseId,
                    domainEvent.Title,
                    domainEvent.Content,
                    domainEvent.Importance
                )
            );
        }
    }
}
