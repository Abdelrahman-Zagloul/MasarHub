using MasarHub.Application.Abstractions.ExternalServices;
using MasarHub.Application.Abstractions.Jobs;
using MasarHub.Application.Common.DomainEvents;
using MasarHub.Domain.Modules.Payments.Events;
using MediatR;

namespace MasarHub.Application.Features.Payments.Events
{
    public sealed class PaymentFailedEmailEventHandler
        : INotificationHandler<DomainEventNotification<PaymentFailedDomainEvent>>
    {
        private readonly IBackgroundJobService _backgroundJobService;

        public PaymentFailedEmailEventHandler(IBackgroundJobService backgroundJobService)
        {
            _backgroundJobService = backgroundJobService;
        }

        public Task Handle(DomainEventNotification<PaymentFailedDomainEvent> notification, CancellationToken cancellationToken)
        {
            _backgroundJobService.Enqueue<IEmailJob>(x =>
                x.SendPaymentFailedEmailAsync(
                    notification.DomainEvent.UserId,
                    notification.DomainEvent.OrderNumber,
                    notification.DomainEvent.Amount,
                    notification.DomainEvent.OrderId
                )
            );
            return Task.CompletedTask;
        }
    }
}
