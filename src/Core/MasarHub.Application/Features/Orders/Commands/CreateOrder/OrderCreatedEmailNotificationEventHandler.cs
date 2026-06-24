using MasarHub.Application.Abstractions.ExternalServices;
using MasarHub.Application.Abstractions.Jobs;
using MasarHub.Application.Common.DomainEvents;
using MasarHub.Domain.Modules.Orders.Events;
using MediatR;

namespace MasarHub.Application.Features.Orders.Commands.CreateOrder
{
    public sealed class OrderCreatedEmailNotificationEventHandler : INotificationHandler<DomainEventNotification<OrderCreatedDomainEvent>>
    {
        private readonly IBackgroundJobService _backgroundJobService;

        public OrderCreatedEmailNotificationEventHandler(IBackgroundJobService backgroundJobService)
        {
            _backgroundJobService = backgroundJobService;
        }

        public async Task Handle(DomainEventNotification<OrderCreatedDomainEvent> notification, CancellationToken cancellationToken)
        {
            var domainEvent = notification.DomainEvent;

            _backgroundJobService.Enqueue<IOrderJob>(x =>
                x.SendCreatedEmailAsync(
                    domainEvent.UserId,
                    domainEvent.OrderNumber,
                    domainEvent.FinalAmount,
                    domainEvent.OrderId
                )
            );
        }
    }
}
