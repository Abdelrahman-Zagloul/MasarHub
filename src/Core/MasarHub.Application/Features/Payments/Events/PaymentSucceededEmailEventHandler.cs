using MasarHub.Application.Abstractions.ExternalServices;
using MasarHub.Application.Abstractions.Jobs;
using MasarHub.Application.Common.DomainEvents;
using MasarHub.Domain.Modules.Payments.Events;
using MediatR;

namespace MasarHub.Application.Features.Payments.Events
{
    public sealed class PaymentSucceededEmailEventHandler
        : INotificationHandler<DomainEventNotification<PaymentSucceededDomainEvent>>
    {
        private readonly IBackgroundJobService _backgroundJobService;

        public PaymentSucceededEmailEventHandler(IBackgroundJobService backgroundJobService)
        {
            _backgroundJobService = backgroundJobService;
        }

        public Task Handle(DomainEventNotification<PaymentSucceededDomainEvent> notification, CancellationToken cancellationToken)
        {

            _backgroundJobService.Enqueue<IEmailJob>(x =>
                x.SendPaymentSucceededEmailAsync(
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
