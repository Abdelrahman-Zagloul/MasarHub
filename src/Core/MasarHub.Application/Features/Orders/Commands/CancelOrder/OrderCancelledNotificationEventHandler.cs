using MasarHub.Application.Abstractions.ExternalServices;
using MasarHub.Application.Abstractions.Jobs;
using MasarHub.Application.Abstractions.Services;
using MasarHub.Application.Common.DomainEvents;
using MasarHub.Application.Common.Extensions;
using MasarHub.Domain.Modules.Notifications;
using MasarHub.Domain.Modules.Orders.Events;
using MediatR;

namespace MasarHub.Application.Features.Orders.Commands.CancelOrder
{
    public sealed class OrderCancelledNotificationEventHandler : INotificationHandler<DomainEventNotification<OrderCancelledDomainEvent>>
    {
        private readonly INotificationRealtimeService _notificationRealtimeService;
        private readonly IBackgroundJobService _backgroundJobService;

        public OrderCancelledNotificationEventHandler(INotificationRealtimeService notificationRealtimeService, IBackgroundJobService backgroundJobService)
        {
            _notificationRealtimeService = notificationRealtimeService;
            _backgroundJobService = backgroundJobService;
        }

        public async Task Handle(DomainEventNotification<OrderCancelledDomainEvent> notification, CancellationToken cancellationToken)
        {
            var domainEvent = notification.DomainEvent;
            var actionUrl = $"/orders/{domainEvent.OrderId}";

            var notificationCreationResult = Notification.CreateForUser(
                userId: domainEvent.UserId,
                title: "Order Cancelled",
                message: $"Your order #{domainEvent.OrderNumber} has been cancelled.",
                type: NotificationType.OrderCancelled,
                priority: NotificationPriority.Normal,
                actionUrl: actionUrl);

            if (notificationCreationResult.IsFailure)
                return;

            await _notificationRealtimeService.SendToUserAsync(
                domainEvent.UserId,
                notificationCreationResult.Value.ToRealtimeResponse(),
                cancellationToken
            );

            _backgroundJobService.Enqueue<ICreateNotificationJob>(x => x.ExecuteAsync(notificationCreationResult.Value.ToCreateRequest()));
        }
    }
}
