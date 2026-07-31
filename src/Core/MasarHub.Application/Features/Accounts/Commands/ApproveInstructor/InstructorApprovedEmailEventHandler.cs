using MasarHub.Application.Abstractions.ExternalServices;
using MasarHub.Application.Abstractions.Jobs;
using MasarHub.Application.Common.DomainEvents;
using MasarHub.Domain.Modules.Profiles.Events;
using MediatR;

namespace MasarHub.Application.Features.Accounts.Commands.ApproveInstructor
{
    public sealed class InstructorApprovedEmailEventHandler
        : INotificationHandler<DomainEventNotification<InstructorApprovedDomainEvent>>
    {
        private readonly IBackgroundJobService _backgroundJobService;

        public InstructorApprovedEmailEventHandler(IBackgroundJobService backgroundJobService)
        {
            _backgroundJobService = backgroundJobService;
        }

        public Task Handle(DomainEventNotification<InstructorApprovedDomainEvent> notification, CancellationToken cancellationToken)
        {
            var domainEvent = notification.DomainEvent;

            _backgroundJobService.Enqueue<IEmailJob>(x =>
                x.SendInstructorApprovedEmailAsync(domainEvent.InstructorUserId));

            return Task.CompletedTask;
        }
    }
}
