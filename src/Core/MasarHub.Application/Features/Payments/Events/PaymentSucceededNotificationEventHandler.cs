using MasarHub.Application.Abstractions.ExternalServices;
using MasarHub.Application.Abstractions.Jobs;
using MasarHub.Application.Abstractions.Services;
using MasarHub.Application.Common.DomainEvents;
using MasarHub.Application.Common.Extensions;
using MasarHub.Domain.Modules.Notifications;
using MasarHub.Domain.Modules.Payments.Events;
using MediatR;

namespace MasarHub.Application.Features.Payments.Events
{
    public sealed class PaymentSucceededNotificationEventHandler
        : INotificationHandler<DomainEventNotification<PaymentSucceededDomainEvent>>
    {
        private readonly INotificationRealtimeService _notificationRealtimeService;
        private readonly IBackgroundJobService _backgroundJobService;

        public PaymentSucceededNotificationEventHandler(INotificationRealtimeService notificationRealtimeService, IBackgroundJobService backgroundJobService)
        {
            _notificationRealtimeService = notificationRealtimeService;
            _backgroundJobService = backgroundJobService;
        }

        public async Task Handle(DomainEventNotification<PaymentSucceededDomainEvent> notification, CancellationToken cancellationToken)
        {
            var domainEvent = notification.DomainEvent;
            var actionUrl = $"/orders/{domainEvent.OrderId}";

            var notificationResult = Notification.CreateForUser(
                userId: domainEvent.UserId,
                title: "Payment Received",
                message: $"Your payment for order #{domainEvent.OrderNumber} has been received successfully.",
                type: NotificationType.PaymentReceived,
                priority: NotificationPriority.High,
                actionUrl: actionUrl);

            if (notificationResult.IsFailure)
                return;

            await _notificationRealtimeService.SendToUserAsync(domainEvent.UserId, notificationResult.Value.ToRealtimeResponse(), cancellationToken);
            _backgroundJobService.Enqueue<ICreateNotificationJob>(x => x.ExecuteAsync(notificationResult.Value.ToCreateRequest()));
        }
    }
}
