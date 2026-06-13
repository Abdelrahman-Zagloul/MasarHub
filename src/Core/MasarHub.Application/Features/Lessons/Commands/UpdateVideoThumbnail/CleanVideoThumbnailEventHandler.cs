using MasarHub.Application.Abstractions.ExternalServices;
using MasarHub.Application.Abstractions.Jobs;
using MasarHub.Application.Common.DomainEvents;
using MasarHub.Domain.Modules.Courses.Events;
using MediatR;

namespace MasarHub.Application.Features.Lessons.Commands.UpdateVideoThumbnail
{
    public sealed class CleanVideoThumbnailEventHandler
        : INotificationHandler<DomainEventNotification<ThumbnailChangedDomainEvent>>
    {
        private readonly IBackgroundJobService _backgroundJobService;

        public CleanVideoThumbnailEventHandler(IBackgroundJobService backgroundJobService)
        {
            _backgroundJobService = backgroundJobService;
        }

        public Task Handle(DomainEventNotification<ThumbnailChangedDomainEvent> notification, CancellationToken cancellationToken)
        {
            _backgroundJobService.Enqueue<ILessonJob>(job =>
                job.CleanUpVideoThumbnailAsync(notification.DomainEvent.OldThumbnailPublicId));

            return Task.CompletedTask;
        }
    }
}
