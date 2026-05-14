namespace MasarHub.Application.Features.Authentication.Commands.TwoFactor.DisableTwoFactor
{
    public sealed record DisableTwoFactorResult
    (
        Guid UserId,
        string FullName,
        string Email
    );
}
