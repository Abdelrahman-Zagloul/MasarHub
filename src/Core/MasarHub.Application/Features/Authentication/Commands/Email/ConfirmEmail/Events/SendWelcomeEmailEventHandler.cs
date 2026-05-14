using MasarHub.Application.Abstractions.ExternalServices;
using MasarHub.Application.Abstractions.Services;
using MasarHub.Application.Common.Constants;
using MediatR;

namespace MasarHub.Application.Features.Authentication.Commands.Email.ConfirmEmail.Events
{
    public sealed class SendWelcomeEmailEventHandler : INotificationHandler<EmailConfirmedEvent>
    {
        private readonly IAppEmailService _appEmailService;
        private readonly IBackgroundJobService _backgroundJobService;

        public SendWelcomeEmailEventHandler(IAppEmailService appEmailService, IBackgroundJobService backgroundJobService)
        {
            _appEmailService = appEmailService;
            _backgroundJobService = backgroundJobService;
        }

        public Task Handle(EmailConfirmedEvent notification, CancellationToken cancellationToken)
        {
            _backgroundJobService.Enqueue(() =>
                _appEmailService.SendWelcomeEmailAsync(notification.fullName, notification.User.Email, notification.User.Roles.FirstOrDefault() ?? Roles.Student));

            return Task.CompletedTask;
        }
    }
}
