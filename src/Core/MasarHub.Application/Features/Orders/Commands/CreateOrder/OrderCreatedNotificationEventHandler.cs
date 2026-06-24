using MasarHub.Application.Abstractions.ExternalServices;
using MasarHub.Application.Abstractions.Jobs;
using MasarHub.Application.Abstractions.Services;
using MasarHub.Application.Common.DomainEvents;
using MasarHub.Application.Common.Extensions;
using MasarHub.Domain.Modules.Notifications;
using MasarHub.Domain.Modules.Orders.Events;
using MediatR;

namespace MasarHub.Application.Features.Orders.Commands.CreateOrder
{
    public sealed class OrderCreatedNotificationEventHandler : INotificationHandler<DomainEventNotification<OrderCreatedDomainEvent>>
    {
        private readonly INotificationRealtimeService _notificationRealtimeService;
        private readonly IBackgroundJobService _backgroundJobService;

        public OrderCreatedNotificationEventHandler(INotificationRealtimeService notificationRealtimeService, IBackgroundJobService backgroundJobService)
        {
            _notificationRealtimeService = notificationRealtimeService;
            _backgroundJobService = backgroundJobService;
        }

        public async Task Handle(DomainEventNotification<OrderCreatedDomainEvent> notification, CancellationToken cancellationToken)
        {
            var domainEvent = notification.DomainEvent;
            var actionUrl = $"/orders/{domainEvent.OrderId}";

            var notificationCreationResult = Notification.CreateForUser(
                userId: domainEvent.UserId,
                title: "Order Created",
                message: $"Your order #{domainEvent.OrderNumber} has been created successfully.",
                type: NotificationType.OrderCreated,
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
