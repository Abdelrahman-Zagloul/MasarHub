using MasarHub.Application.Features.Authentication.Commands.Password.ChangePassword;
using MediatR;

namespace MasarHub.Application.Features.Authentication.Commands.Password.ChangePassword.Events
{
    public sealed record PasswordChangedEvent(PasswordChangedResult PasswordChangedResult) : INotification;
}
