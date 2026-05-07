using MasarHub.Application.Abstractions.ExternalServices;
using MasarHub.Application.Abstractions.Services;
using MediatR;

namespace MasarHub.Application.Features.Authentication.Commands.RegisterStudent.Events
{
    public sealed class SendStudentVerificationEmailHandler : INotificationHandler<StudentRegisteredEvent>
    {
        private readonly IAppEmailService _appEmailService;
        private readonly IBackgroundJobService _backgroundJobService;
        public SendStudentVerificationEmailHandler(IAppEmailService appEmailService, IBackgroundJobService backgroundJobService)
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
