using MasarHub.Application.Abstractions.ExternalServices;
using MasarHub.Application.Abstractions.Services;
using MediatR;

namespace MasarHub.Application.Features.Authentication.Commands.Email.ResendConfirmEmail.Events
{
    public sealed class ResendConfirmEmailEventHandler : INotificationHandler<ConfirmEmailTokenCreatedEvent>
    {
        private readonly IAppEmailService _appEmailService;
        private readonly IBackgroundJobService _backgroundJobService;

        public ResendConfirmEmailEventHandler(IAppEmailService appEmailService, IBackgroundJobService backgroundJobService)
        {
            _appEmailService = appEmailService;
            _backgroundJobService = backgroundJobService;
        }

        public Task Handle(ConfirmEmailTokenCreatedEvent notification, CancellationToken cancellationToken)
        {
            var encodedToken = Uri.EscapeDataString(notification.TokenResult.Token);

            _backgroundJobService.Enqueue(() => _appEmailService.SendConfirmEmailAsync(
                        notification.TokenResult.FullName,
                        notification.TokenResult.Email,
                        encodedToken)
            );

            return Task.CompletedTask;
        }
    }
}
