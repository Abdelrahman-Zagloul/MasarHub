using MasarHub.Application.Abstractions.ExternalServices;
using MasarHub.Application.Abstractions.Services;
using MediatR;

namespace MasarHub.Application.Features.Authentication.Commands.TwoFactor.EnableTwoFactor.Events
{
    public sealed class SendTwoFactorEnabledEmailEventHandler : INotificationHandler<TwoFactorEnabledEvent>
    {
        private readonly IAppEmailService _appEmailService;
        private readonly IBackgroundJobService _backgroundJobService;

        public SendTwoFactorEnabledEmailEventHandler(IAppEmailService appEmailService, IBackgroundJobService backgroundJobService)
        {
            _appEmailService = appEmailService;
            _backgroundJobService = backgroundJobService;
        }

        public async Task Handle(TwoFactorEnabledEvent notification, CancellationToken cancellationToken)
        {
            _backgroundJobService.Enqueue(() =>
                _appEmailService.SendTwoFactorEnabledEmailAsync(
                    notification.Result.FullName,
                    notification.Result.Email,
                    notification.Result.Provider
                )
            );
        }
    }
}