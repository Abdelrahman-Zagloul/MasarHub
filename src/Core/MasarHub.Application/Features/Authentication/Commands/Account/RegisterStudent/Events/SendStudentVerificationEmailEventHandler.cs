using MasarHub.Application.Abstractions.ExternalServices;
using MasarHub.Application.Abstractions.Services;
using MediatR;

namespace MasarHub.Application.Features.Authentication.Commands.Account.RegisterStudent.Events
{
    public sealed class SendStudentVerificationEmailEventHandler : INotificationHandler<StudentRegisteredEvent>
    {
        private readonly IAppEmailService _appEmailService;
        private readonly IBackgroundJobService _backgroundJobService;
        public SendStudentVerificationEmailEventHandler(IAppEmailService appEmailService, IBackgroundJobService backgroundJobService)
        {
            _appEmailService = appEmailService;
            _backgroundJobService = backgroundJobService;
        }

        public async Task Handle(StudentRegisteredEvent notification, CancellationToken cancellationToken)
        {
            var encodedToken = Uri.EscapeDataString(notification.EmailVerificationToken);

            _backgroundJobService.Enqueue(() =>
            _appEmailService.SendConfirmEmailAsync(notification.FullName, notification.Email, encodedToken));
        }
    }
}
