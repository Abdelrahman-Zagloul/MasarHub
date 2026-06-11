using MasarHub.Application.Abstractions.ExternalServices;
using MasarHub.Application.Abstractions.Jobs;
using MasarHub.Application.Common.DomainEvents;
using MasarHub.Domain.Modules.Courses.Events;
using MediatR;

namespace MasarHub.Application.Features.Modules.Commands.CreateModule
{
    public sealed class CreateAnnouncementOnModuleCreatedEventHandler
        : INotificationHandler<DomainEventNotification<ModuleCreatedDomainEvent>>
    {
        private readonly IBackgroundJobService _backgroundJobService;

        public CreateAnnouncementOnModuleCreatedEventHandler(IBackgroundJobService backgroundJobService)
        {
            _backgroundJobService = backgroundJobService;
        }

        public Task Handle(DomainEventNotification<ModuleCreatedDomainEvent> notification, CancellationToken cancellationToken)
        {
            _backgroundJobService.Enqueue<IModuleJob>(job =>
                job.CreateAnnouncementForNewModuleAsync(
                    notification.DomainEvent.CourseId,
                    notification.DomainEvent.ModuleTitle
                )
            );
            return Task.CompletedTask;
        }
    }
}
