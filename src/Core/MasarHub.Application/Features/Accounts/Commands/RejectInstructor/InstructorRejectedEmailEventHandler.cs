using MasarHub.Application.Abstractions.ExternalServices;
using MasarHub.Application.Abstractions.Jobs;
using MasarHub.Application.Common.DomainEvents;
using MasarHub.Domain.Modules.Profiles.Events;
using MediatR;

namespace MasarHub.Application.Features.Accounts.Commands.RejectInstructor
{
    public sealed class InstructorRejectedEmailEventHandler
        : INotificationHandler<DomainEventNotification<InstructorRejectedDomainEvent>>
    {
        private readonly IBackgroundJobService _backgroundJobService;

        public InstructorRejectedEmailEventHandler(IBackgroundJobService backgroundJobService)
        {
            _backgroundJobService = backgroundJobService;
        }

        public Task Handle(DomainEventNotification<InstructorRejectedDomainEvent> notification, CancellationToken cancellationToken)
        {
            var domainEvent = notification.DomainEvent;

            _backgroundJobService.Enqueue<IEmailJob>(x =>
                x.SendInstructorRejectedEmailAsync(domainEvent.InstructorUserId, domainEvent.Reason));

            return Task.CompletedTask;
        }
    }
}
