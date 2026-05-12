using MediatR;

namespace MasarHub.Application.Features.Authentication.Commands.ForgetPassword.Events
{
    public sealed record PasswordResetRequestedEvent(ForgetPasswordResult ForgetPasswordResult) : INotification;
}
