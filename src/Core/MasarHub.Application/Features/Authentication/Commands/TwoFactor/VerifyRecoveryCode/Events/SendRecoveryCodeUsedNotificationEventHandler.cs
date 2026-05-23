using MasarHub.Application.Abstractions.ExternalServices;
using MasarHub.Application.Abstractions.Jobs;
using MasarHub.Application.Abstractions.Services;
using MasarHub.Application.Common.Extensions;
using MasarHub.Domain.Modules.Notifications;
using MediatR;

namespace MasarHub.Application.Features.Authentication.Commands.TwoFactor.VerifyRecoveryCode.Events
{
    public sealed class SendRecoveryCodeUsedNotificationEventHandler : INotificationHandler<TwoFactorRecoveryCodeUsedEvent>
    {
        private readonly INotificationRealtimeService _notificationRealtimeService;
        private readonly IBackgroundJobService _backgroundJobService;

        public SendRecoveryCodeUsedNotificationEventHandler(
            INotificationRealtimeService notificationRealtimeService,
            IBackgroundJobService backgroundJobService)
        {
            _notificationRealtimeService = notificationRealtimeService;
            _backgroundJobService = backgroundJobService;
        }

        public async Task Handle(TwoFactorRecoveryCodeUsedEvent notification, CancellationToken cancellationToken)
        {
            var notificationResult = Notification.CreateForUser(
                userId: notification.User.Id,
                title: "Recovery Code Used",
                message: "A recovery code was used to sign in to your account.",
                type: NotificationType.RecoveryCodeUsed,
                priority: NotificationPriority.High,
                actionUrl: "/settings/security");

            if (notificationResult.IsFailure)
                return;

            await _notificationRealtimeService.SendToUserAsync(
                notification.User.Id,
                notificationResult.Value.ToRealtimeResponse(),
                cancellationToken);

            _backgroundJobService.Enqueue<ICreateNotificationJob>(x =>
                x.ExecuteAsync(notificationResult.Value.ToCreateRequest()));
        }
    }
}
