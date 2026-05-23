using MasarHub.Application.Abstractions.ExternalServices;
using MasarHub.Application.Abstractions.Jobs;
using MasarHub.Application.Abstractions.Services;
using MasarHub.Application.Common.Extensions;
using MasarHub.Domain.Modules.Notifications;
using MediatR;

namespace MasarHub.Application.Features.Authentication.Commands.TwoFactor.DisableTwoFactor.Events
{
    public sealed class SendTwoFactorDisabledNotificationEventHandler
        : INotificationHandler<TwoFactorDisabledEvent>
    {
        private readonly INotificationRealtimeService _notificationRealtimeService;
        private readonly IBackgroundJobService _backgroundJobService;

        public SendTwoFactorDisabledNotificationEventHandler(INotificationRealtimeService notificationRealtimeService, IBackgroundJobService backgroundJobService)
        {
            _notificationRealtimeService = notificationRealtimeService;
            _backgroundJobService = backgroundJobService;
        }

        public async Task Handle(TwoFactorDisabledEvent notification, CancellationToken cancellationToken)
        {
            var notificationResult = Notification.CreateForUser(
                userId: notification.Result.UserId,
                title: "Two-Factor Authentication Disabled",
                message: "Two-factor authentication has been disabled for your account.",
                type: NotificationType.TwoFactorDisabled,
                priority: NotificationPriority.High,
                actionUrl: "/settings/security");

            if (notificationResult.IsFailure)
                return;

            await _notificationRealtimeService.SendToUserAsync(
                notification.Result.UserId,
                notificationResult.Value.ToRealtimeResponse(),
                cancellationToken
            );

            _backgroundJobService.Enqueue<ICreateNotificationJob>(x =>
                    x.ExecuteAsync(notificationResult.Value.ToCreateRequest()));
        }
    }
}
