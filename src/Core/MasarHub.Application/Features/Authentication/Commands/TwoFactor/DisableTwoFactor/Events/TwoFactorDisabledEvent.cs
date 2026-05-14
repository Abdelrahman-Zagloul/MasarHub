using MediatR;

namespace MasarHub.Application.Features.Authentication.Commands.TwoFactor.DisableTwoFactor.Events
{
    public sealed record TwoFactorDisabledEvent(DisableTwoFactorResult Result) : INotification;
}
