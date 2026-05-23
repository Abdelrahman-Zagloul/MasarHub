using MasarHub.Application.Abstractions.ExternalServices;
using MasarHub.Application.Abstractions.Jobs;
using MasarHub.Application.Abstractions.Services;
using MasarHub.Domain.Modules.Notifications;
using MediatR;

namespace MasarHub.Application.Features.Authentication.Commands.Email.ConfirmEmail.Events
{
    public sealed class SendWelcomeNotificationEventHandler : INotificationHandler<EmailConfirmedEvent>
    {
        private readonly INotificationRealtimeService _notificationRealtimeService;
        private readonly IBackgroundJobService _backgroundJobService;

        public SendWelcomeNotificationEventHandler(INotificationRealtimeService notificationRealtimeService, IBackgroundJobService backgroundJobService)
        {
            _notificationRealtimeService = notificationRealtimeService;
            _backgroundJobService = backgroundJobService;
        }

        public async Task Handle(EmailConfirmedEvent notification, CancellationToken cancellationToken)
        {
            var actionUrl = "/dashboard";
            var notificationResult = Notification.CreateForUser(
                userId: notification.User.Id,
                title: "Welcome to MasarHub",
                message: "Your account has been verified successfully.",
                type: NotificationType.EmailConfirmation,
                priority: NotificationPriority.Normal,
                actionUrl: actionUrl);

            if (notificationResult.IsFailure)
                return;

            await _notificationRealtimeService.SendToUserAsync(notification.User.Id,
            new
            {
                notificationResult.Value.Id,
                notificationResult.Value.Title,
                notificationResult.Value.Message,
                notificationResult.Value.Type,
                notificationResult.Value.Priority,
                notificationResult.Value.ActionUrl,
                notificationResult.Value.CreatedAt
            },
            cancellationToken);

            _backgroundJobService.Enqueue<ICreateNotificationJob>(x =>
                x.ExecuteAsync(CreateNotificationRequest.ForUser(notificationResult.Value)));
        }
    }
}