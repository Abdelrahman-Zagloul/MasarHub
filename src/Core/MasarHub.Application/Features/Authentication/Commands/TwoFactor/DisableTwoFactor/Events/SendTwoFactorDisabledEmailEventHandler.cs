using MasarHub.Application.Abstractions.ExternalServices;
using MasarHub.Application.Abstractions.Services;
using MediatR;

namespace MasarHub.Application.Features.Authentication.Commands.TwoFactor.DisableTwoFactor.Events
{
    public sealed class SendTwoFactorDisabledEmailEventHandler : INotificationHandler<TwoFactorDisabledEvent>
    {
        private readonly IBackgroundJobService _backgroundJobService;
        private readonly IAppEmailService _appEmailService;

        public SendTwoFactorDisabledEmailEventHandler(IBackgroundJobService backgroundJobService, IAppEmailService appEmailService)
        {
            _backgroundJobService = backgroundJobService;
            _appEmailService = appEmailService;
        }

        public Task Handle(TwoFactorDisabledEvent notification, CancellationToken cancellationToken)
        {
            _backgroundJobService.Enqueue(() =>
                _appEmailService.SendTwoFactorDisabledEmailAsync(
                    notification.Result.FullName,
                    notification.Result.Email));

            return Task.CompletedTask;
        }
    }
}
