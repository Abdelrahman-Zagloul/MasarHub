using MasarHub.Application.Abstractions.Identity;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Application.Features.Authentication.Shared;
using MasarHub.Domain.Modules.Profiles;
using MasarHub.Infrastructure.Persistence.Identity;
using Microsoft.AspNetCore.Identity;

namespace MasarHub.Infrastructure.Identity.TwoFactor.Providers
{
    public sealed class AuthenticatorTwoFactorProvider : ITwoFactorProvider
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthenticatorTwoFactorProvider(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public TwoFactorProvider Provider => TwoFactorProvider.Authenticator;
        public Task<Result> SendCodeAsync(Guid userId, CancellationToken ct = default)
        {
            return Task.FromResult(Result.Success());
        }

        public async Task<Result<TokenUser>> VerifyCodeAsync(Guid userId, string code, CancellationToken ct = default)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return Error.NotFound("user.not_found");

            var verificationResult = await _userManager.VerifyTwoFactorTokenAsync(user, TokenOptions.DefaultAuthenticatorProvider, code);
            if (!verificationResult)
                return Error.BadRequest("auth.2fa_verification_failed");

            var roles = await _userManager.GetRolesAsync(user);
            return new TokenUser(userId, user.Email!, roles);
        }
    }
}
