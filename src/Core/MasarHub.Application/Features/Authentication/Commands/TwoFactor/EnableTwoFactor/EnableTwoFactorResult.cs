using MasarHub.Domain.Modules.Profiles;

namespace MasarHub.Application.Features.Authentication.Commands.TwoFactor.EnableTwoFactor
{
    public sealed record EnableTwoFactorResult
    (
       Guid UserId,
       string FullName,
       string Email,
       TwoFactorProvider Provider
    );
}
