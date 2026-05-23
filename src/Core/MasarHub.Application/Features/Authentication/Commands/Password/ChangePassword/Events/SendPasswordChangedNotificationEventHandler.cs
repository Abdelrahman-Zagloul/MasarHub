using MasarHub.Application.Abstractions.ExternalServices;
using MasarHub.Application.Abstractions.Jobs;
using MasarHub.Application.Abstractions.Services;
using MasarHub.Domain.Modules.Notifications;
using MediatR;

namespace MasarHub.Application.Features.Authentication.Commands.Password.ChangePassword.Events
{
    public sealed class SendPasswordChangedNotificationEventHandler : INotificationHandler<PasswordChangedEvent>
    {
        private readonly INotificationRealtimeService _notificationRealtimeService;
        private readonly IBackgroundJobService _backgroundJobService;

        public SendPasswordChangedNotificationEventHandler(INotificationRealtimeService notificationRealtimeService, IBackgroundJobService backgroundJobService)
        {
            _notificationRealtimeService = notificationRealtimeService;
            _backgroundJobService = backgroundJobService;
        }

        public async Task Handle(PasswordChangedEvent notification, CancellationToken cancellationToken)
        {
            var notificationResult = Notification.CreateForUser(
                userId: notification.PasswordChangedResult.UserId,
                title: "Password Changed",
                message: "Your password has been changed successfully.",
                type: NotificationType.PasswordChanged,
                priority: NotificationPriority.High,
                actionUrl: "/settings/security");

            if (notificationResult.IsFailure)
                return;

            await _notificationRealtimeService.SendToUserAsync(
                notification.PasswordChangedResult.UserId,
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

            _backgroundJobService.Enqueue<ICreateNotificationJob>(x => x.ExecuteAsync(notificationResult.Value));
        }
    }
}
