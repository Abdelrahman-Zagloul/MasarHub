using MediatR;

namespace MasarHub.Application.Features.Authentication.Commands.ChangePassword.Events
{
    public sealed record PasswordChangedEvent(PasswordChangedResult PasswordChangedResult) : INotification;
}
