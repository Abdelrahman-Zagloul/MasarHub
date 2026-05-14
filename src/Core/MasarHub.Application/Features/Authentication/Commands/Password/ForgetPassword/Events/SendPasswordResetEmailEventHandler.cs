using MasarHub.Application.Abstractions.ExternalServices;
using MasarHub.Application.Abstractions.Services;
using MediatR;

namespace MasarHub.Application.Features.Authentication.Commands.Password.ForgetPassword.Events
{
    public sealed class SendPasswordResetEmailEventHandler : INotificationHandler<PasswordResetRequestedEvent>
    {
        private readonly IAppEmailService _appEmailService;
        private readonly IBackgroundJobService _backgroundJobService;

        public SendPasswordResetEmailEventHandler(IAppEmailService appEmailService, IBackgroundJobService backgroundJobService)
        {
            _appEmailService = appEmailService;
            _backgroundJobService = backgroundJobService;
        }

        public Task Handle(PasswordResetRequestedEvent notification, CancellationToken cancellationToken)
        {
            var encodedToke = Uri.EscapeDataString(notification.ForgetPasswordResult.Token);

            _backgroundJobService.Enqueue(() =>
                _appEmailService.SendPasswordResetEmailAsync(
                    notification.ForgetPasswordResult.FullName,
                    notification.ForgetPasswordResult.Email,
                    encodedToke));

            return Task.CompletedTask;
        }
    }
}
