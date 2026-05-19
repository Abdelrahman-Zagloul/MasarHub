using MasarHub.Application.Features.Authentication.Shared;
using MasarHub.Domain.Modules.Profiles;

namespace MasarHub.Application.Features.Authentication.Commands.Account.Login
{
    public sealed record LoginResult
    (
        bool RequiresTwoFactor,
        AccessWithRefreshTokenResult? Tokens,
         Guid? ChallengeId,
         TwoFactorProvider? Provider
    )
    {
        public static LoginResult Success(AccessWithRefreshTokenResult tokens)
            => new(false, tokens, null, null);

        public static LoginResult TwoFactorRequired(Guid challengeId, TwoFactorProvider provider)
            => new(true, null, challengeId, provider);
    }


    public sealed record AuthenticateUserResult(bool RequiresTwoFactor, TokenUser TokenUser, TwoFactorProvider? Provider);

}
