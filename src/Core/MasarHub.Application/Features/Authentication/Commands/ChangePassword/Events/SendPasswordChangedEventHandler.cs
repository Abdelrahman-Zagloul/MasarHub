using MasarHub.Application.Abstractions.ExternalServices;
using MasarHub.Application.Abstractions.Services;
using MediatR;

namespace MasarHub.Application.Features.Authentication.Commands.ChangePassword.Events
{
    public sealed class SendPasswordChangedEventHandler : INotificationHandler<PasswordChangedEvent>
    {
        private readonly IAppEmailService _appEmailService;
        private readonly IBackgroundJobService _backgroundJobService;

        public SendPasswordChangedEventHandler(IAppEmailService emailService, IBackgroundJobService backgroundJobService)
        {
            _appEmailService = emailService;
            _backgroundJobService = backgroundJobService;
        }

        public Task Handle(PasswordChangedEvent notification, CancellationToken cancellationToken)
        {
            _backgroundJobService.Enqueue(() =>
                _appEmailService.SendPasswordChangedEmailAsync(
                    notification.PasswordChangedResult.FullName,
                    notification.PasswordChangedResult.Email)
            );

            return Task.CompletedTask;
        }
    }
}
