using MasarHub.Application.Abstractions.ExternalServices;
using MasarHub.Application.Abstractions.Services;
using MediatR;

namespace MasarHub.Application.Features.Authentication.Commands.RegisterStudent
{
    public sealed class SendVerificationEmailHandler : INotificationHandler<StudentRegisteredEvent>
    {
        private readonly IAppEmailService _appEmailService;
        private readonly IBackgroundJobService _backgroundJobService;
        public SendVerificationEmailHandler(IAppEmailService appEmailService, IBackgroundJobService backgroundJobService)
        {
            _appEmailService = appEmailService;
            _backgroundJobService = backgroundJobService;
        }

        public async Task Handle(StudentRegisteredEvent notification, CancellationToken cancellationToken)
        {
            var encodedToken = Uri.EscapeDataString(notification.Token);

            _backgroundJobService.Enqueue(() =>
            _appEmailService.SendConfirmEmailAsync(notification.FullName, notification.Email, encodedToken));
        }
    }
}
