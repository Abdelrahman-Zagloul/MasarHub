using MasarHub.Application.Abstractions.ExternalServices;
using MasarHub.Application.Abstractions.Services;
using MediatR;

namespace MasarHub.Application.Features.Authentication.Commands.Account.RegisterInstructor.Events
{
    public class SendInstructorVerificationEmailEventHandler : INotificationHandler<InstructorRegisteredEvent>
    {
        private readonly IAppEmailService _appEmailService;
        private readonly IBackgroundJobService _backgroundJobService;

        public SendInstructorVerificationEmailEventHandler(IAppEmailService appEmailService, IBackgroundJobService backgroundJobService)
        {
            _appEmailService = appEmailService;
            _backgroundJobService = backgroundJobService;
        }

        public async Task Handle(InstructorRegisteredEvent notification, CancellationToken cancellationToken)
        {
            var encodedToken = Uri.EscapeDataString(notification.EmailVerificationToken);

            _backgroundJobService.Enqueue(() =>
                    _appEmailService.SendConfirmEmailAsync(notification.FullName, notification.Email, encodedToken));
        }
    }
}
