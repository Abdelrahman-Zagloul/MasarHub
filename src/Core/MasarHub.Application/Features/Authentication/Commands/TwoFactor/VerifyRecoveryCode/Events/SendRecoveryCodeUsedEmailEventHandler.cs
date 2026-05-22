using MasarHub.Application.Abstractions.ExternalServices;
using MasarHub.Application.Abstractions.Services;
using MediatR;

namespace MasarHub.Application.Features.Authentication.Commands.TwoFactor.VerifyRecoveryCode.Events
{
    public sealed class SendRecoveryCodeUsedEmailEventHandler : INotificationHandler<TwoFactorRecoveryCodeUsedEvent>
    {
        private readonly IAppEmailService _appEmailService;
        private readonly IBackgroundJobService _backgroundJobService;

        public SendRecoveryCodeUsedEmailEventHandler(IAppEmailService appEmailService, IBackgroundJobService backgroundJobService)
        {
            _appEmailService = appEmailService;
            _backgroundJobService = backgroundJobService;
        }

        public async Task Handle(TwoFactorRecoveryCodeUsedEvent notification, CancellationToken cancellationToken)
        {
            _backgroundJobService.Enqueue(() =>
                _appEmailService.SendRecoveryCodeUsedEmailAsync(
                    notification.User.FullName,
                    notification.User.Email
                )
            );
        }
    }
}
