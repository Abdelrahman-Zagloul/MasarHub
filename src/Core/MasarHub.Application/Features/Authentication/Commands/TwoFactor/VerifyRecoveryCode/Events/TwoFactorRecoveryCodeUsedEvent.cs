using MasarHub.Application.Features.Authentication.Shared;
using MediatR;

namespace MasarHub.Application.Features.Authentication.Commands.TwoFactor.VerifyRecoveryCode.Events
{
    public sealed record TwoFactorRecoveryCodeUsedEvent(TokenUser User) : INotification;
}
