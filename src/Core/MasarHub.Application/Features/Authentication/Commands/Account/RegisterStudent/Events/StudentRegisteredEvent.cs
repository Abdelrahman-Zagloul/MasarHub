using MediatR;

namespace MasarHub.Application.Features.Authentication.Commands.Account.RegisterStudent.Events
{
    public sealed record StudentRegisteredEvent(string FullName, string Email, string EmailVerificationToken) : INotification;
}
