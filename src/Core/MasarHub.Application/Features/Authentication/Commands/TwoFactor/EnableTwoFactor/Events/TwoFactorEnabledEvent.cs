using MediatR;

namespace MasarHub.Application.Features.Authentication.Commands.TwoFactor.EnableTwoFactor.Events
{
    public sealed record TwoFactorEnabledEvent(EnableTwoFactorResult Result) : INotification;
}
