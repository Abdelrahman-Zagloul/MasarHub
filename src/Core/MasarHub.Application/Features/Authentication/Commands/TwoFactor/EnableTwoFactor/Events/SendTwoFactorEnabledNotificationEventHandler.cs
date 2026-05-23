using MasarHub.Application.Abstractions.ExternalServices;
using MasarHub.Application.Abstractions.Jobs;
using MasarHub.Application.Abstractions.Services;
using MasarHub.Application.Common.Extensions;
using MasarHub.Domain.Modules.Notifications;
using MediatR;

namespace MasarHub.Application.Features.Authentication.Commands.TwoFactor.EnableTwoFactor.Events
{
    public sealed class SendTwoFactorEnabledNotificationEventHandler
        : INotificationHandler<TwoFactorEnabledEvent>
    {
        private readonly INotificationRealtimeService _notificationRealtimeService;
        private readonly IBackgroundJobService _backgroundJobService;

        public SendTwoFactorEnabledNotificationEventHandler(INotificationRealtimeService notificationRealtimeService, IBackgroundJobService backgroundJobService)
        {
            _notificationRealtimeService = notificationRealtimeService;
            _backgroundJobService = backgroundJobService;
        }

        public async Task Handle(TwoFactorEnabledEvent notification, CancellationToken cancellationToken)
        {
            var notificationResult = Notification.CreateForUser(
                userId: notification.Result.UserId,
                title: "Two-Factor Authentication Enabled",
                message: "Your account is now protected with two-factor authentication.",
                type: NotificationType.TwoFactorEnabled,
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