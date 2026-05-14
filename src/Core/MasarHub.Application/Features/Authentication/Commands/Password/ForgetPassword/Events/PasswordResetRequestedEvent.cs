using MasarHub.Application.Features.Authentication.Commands.Password.ForgetPassword;
using MediatR;

namespace MasarHub.Application.Features.Authentication.Commands.Password.ForgetPassword.Events
{
    public sealed record PasswordResetRequestedEvent(ForgetPasswordResult ForgetPasswordResult) : INotification;
}
