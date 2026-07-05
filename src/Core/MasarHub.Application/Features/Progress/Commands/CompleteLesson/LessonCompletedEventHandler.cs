using MasarHub.Application.Abstractions.ExternalServices;
using MasarHub.Application.Abstractions.Jobs;
using MasarHub.Application.Common.DomainEvents;
using MasarHub.Domain.Modules.Courses.Events;
using MediatR;

namespace MasarHub.Application.Features.Progress.Commands.CompleteLesson
{
    public sealed class LessonCompletedEventHandler : INotificationHandler<DomainEventNotification<LessonProgressCreatedDomainEvent>>
    {
        private readonly IBackgroundJobService _backgroundJobService;
        public LessonCompletedEventHandler(IBackgroundJobService backgroundJobService)
        {
            _backgroundJobService = backgroundJobService;
        }

        public Task Handle(DomainEventNotification<LessonProgressCreatedDomainEvent> notification, CancellationToken cancellationToken)
        {

            _backgroundJobService.Enqueue<IProgressJob>(job =>
                job.UpdateProgressAsync(
                    notification.DomainEvent.UserId,
                    notification.DomainEvent.CourseId,
                    notification.DomainEvent.ModuleId,
                    notification.DomainEvent.LessonId
                )
            );

            return Task.CompletedTask;
        }
    }
}
