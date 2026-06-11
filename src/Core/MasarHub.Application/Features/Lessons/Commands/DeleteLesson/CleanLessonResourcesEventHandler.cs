using MasarHub.Application.Abstractions.ExternalServices;
using MasarHub.Application.Abstractions.Jobs;
using MasarHub.Application.Common.DomainEvents;
using MasarHub.Domain.Modules.Courses.Events;
using MediatR;

namespace MasarHub.Application.Features.Lessons.Commands.DeleteLesson
{
    public sealed class CleanLessonResourcesEventHandler
        : INotificationHandler<DomainEventNotification<LessonDeletedDomainEvent>>
    {
        private readonly IBackgroundJobService _backgroundJobService;
        public CleanLessonResourcesEventHandler(IBackgroundJobService backgroundJobService)
        {
            _backgroundJobService = backgroundJobService;
        }

        public Task Handle(DomainEventNotification<LessonDeletedDomainEvent> notification, CancellationToken cancellationToken)
        {
            _backgroundJobService.Enqueue<ILessonJob>(job =>
                job.CleanUpLessonResourseAsync(notification.DomainEvent.ModuleId, notification.DomainEvent.LessonId));


            return Task.CompletedTask;
        }
    }
}
