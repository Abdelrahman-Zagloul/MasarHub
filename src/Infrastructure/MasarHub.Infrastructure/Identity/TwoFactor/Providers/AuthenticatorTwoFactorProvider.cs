using MasarHub.Application.Abstractions.Identity;
using MasarHub.Application.Common.Results;
using MasarHub.Domain.Modules.Profiles;

namespace MasarHub.Infrastructure.Identity.TwoFactor.Providers
{
    public sealed class AuthenticatorTwoFactorProvider : ITwoFactorProvider
    {
        public TwoFactorProvider Provider => TwoFactorProvider.Authenticator;
        public Task<Result> SendCodeAsync(Guid userId, CancellationToken ct = default)
        {
            return Task.FromResult(Result.Success());
        }
    }
}
