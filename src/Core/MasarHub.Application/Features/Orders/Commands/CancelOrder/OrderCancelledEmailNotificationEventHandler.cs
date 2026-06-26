using MasarHub.Application.Abstractions.ExternalServices;
using MasarHub.Application.Abstractions.Jobs;
using MasarHub.Application.Common.DomainEvents;
using MasarHub.Domain.Modules.Orders.Events;
using MediatR;

namespace MasarHub.Application.Features.Orders.Commands.CancelOrder
{
    public sealed class OrderCancelledEmailNotificationEventHandler : INotificationHandler<DomainEventNotification<OrderCancelledDomainEvent>>
    {
        private readonly IBackgroundJobService _backgroundJobService;

        public OrderCancelledEmailNotificationEventHandler(IBackgroundJobService backgroundJobService)
        {
            _backgroundJobService = backgroundJobService;
        }

        public async Task Handle(DomainEventNotification<OrderCancelledDomainEvent> notification, CancellationToken cancellationToken)
        {
            var domainEvent = notification.DomainEvent;

            _backgroundJobService.Enqueue<IEmailJob>(x =>
                x.SendOrderCancelledEmailAsync(domainEvent.UserId, domainEvent.OrderNumber, domainEvent.OrderId));
        }
    }
}
