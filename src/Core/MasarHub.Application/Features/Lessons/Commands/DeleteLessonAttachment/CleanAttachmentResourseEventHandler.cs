using MasarHub.Application.Abstractions.ExternalServices;
using MasarHub.Application.Abstractions.Jobs;
using MasarHub.Application.Common.DomainEvents;
using MasarHub.Domain.Modules.Courses.Events;
using MediatR;

namespace MasarHub.Application.Features.Lessons.Commands.DeleteLessonAttachment
{
    public sealed class CleanAttachmentResourseEventHandler
        : INotificationHandler<DomainEventNotification<AttachmentDeletedDomainEvent>>
    {
        private readonly IBackgroundJobService _backgroundJobService;

        public CleanAttachmentResourseEventHandler(IBackgroundJobService backgroundJobService)
        {
            _backgroundJobService = backgroundJobService;
        }

        public Task Handle(DomainEventNotification<AttachmentDeletedDomainEvent> notification, CancellationToken cancellationToken)
        {
            _backgroundJobService.Enqueue<ILessonJob>(job =>
                job.CleanUpAttachmentResourseAsync(notification.DomainEvent.AttachmentId));
            return Task.CompletedTask;
        }
    }
}
