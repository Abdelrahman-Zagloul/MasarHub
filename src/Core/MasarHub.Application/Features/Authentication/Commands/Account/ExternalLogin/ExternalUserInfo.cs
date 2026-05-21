using MasarHub.Application.Features.Authentication.Shared;

namespace MasarHub.Application.Features.Authentication.Commands.Account.ExternalLogin
{
    public sealed record ExternalUserInfo
    (
        string Email,
        string FullName,
        string ProviderUserId,
        ExternalLoginProvider Provider
    );
    public sealed record ExternalLoginResult(TokenUser User, string fullName, bool IsNew);
}
