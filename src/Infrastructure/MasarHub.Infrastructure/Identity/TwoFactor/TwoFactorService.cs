using MasarHub.Application.Abstractions.Identity;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Application.Features.Authentication.Commands.TwoFactor.DisableTwoFactor;
using MasarHub.Application.Features.Authentication.Commands.TwoFactor.EnableTwoFactor;
using MasarHub.Application.Features.Authentication.Commands.TwoFactor.SetupAuthenticator;
using MasarHub.Application.Features.Authentication.Shared;
using MasarHub.Domain.Modules.Profiles;
using MasarHub.Infrastructure.Persistence.Identity;
using Microsoft.AspNetCore.Identity;

namespace MasarHub.Infrastructure.Identity.TwoFactor
{
    public sealed class TwoFactorService : ITwoFactorService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITwoFactorChallengeStore _twoFactorChallengeStore;
        private readonly IEnumerable<ITwoFactorProvider> _twoFactorProviders;

        public TwoFactorService(UserManager<ApplicationUser> userManager, ITwoFactorChallengeStore twoFactorChallengeStore, IEnumerable<ITwoFactorProvider> twoFactorProviders)
        {
            _userManager = userManager;
            _twoFactorChallengeStore = twoFactorChallengeStore;
            _twoFactorProviders = twoFactorProviders;
        }

        public async Task<Result<EnableTwoFactorResult>> EnableAsync(Guid userId, TwoFactorProvider provider)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return Error.NotFound("user.not_found");

            if (user.TwoFactorEnabled)
                return Error.Conflict("auth.2fa_already_enabled");

            if (provider == TwoFactorProvider.Authenticator)
                return Error.BadRequest("auth.2fa_use_authenticator_setup");

            user.EnableTwoFactor(provider);

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return Error.Failure("auth.2fa_enable_failed");

            return new EnableTwoFactorResult(user.Id, user.FullName, user.Email!, user.PreferredTwoFactorProvider!.Value);
        }

        public async Task<Result<DisableTwoFactorResult>> DisableAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return Error.NotFound("user.not_found");

            if (!user.TwoFactorEnabled)
                return Error.Conflict("auth.2fa_already_disabled");

            user.DisableTwoFactor();
            await _userManager.ResetAuthenticatorKeyAsync(user);

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return Error.Failure("auth.2fa_disable_failed");

            return new DisableTwoFactorResult(user.Id, user.FullName, user.Email!);
        }

        public async Task<Result> SendCodeAsync(Guid challengeId, CancellationToken ct = default)
        {
            var challengeData = await _twoFactorChallengeStore.GetAsync(challengeId, ct);
            if (challengeData == null)
                return Error.BadRequest("auth.invalid_2fa_challenge");

            var provider = _twoFactorProviders.First(x => x.Provider == challengeData.Provider);

            return await provider.SendCodeAsync(challengeData.UserId);
        }

        public async Task<Result<TokenUser>> VerifyCodeAsync(Guid challengeId, string code, CancellationToken ct = default)
        {
            var challengeData = await _twoFactorChallengeStore.GetAsync(challengeId);
            if (challengeData == null)
                return Error.BadRequest("auth.invalid_2fa_challenge");

            var provider = _twoFactorProviders.First(x => x.Provider == challengeData.Provider);
            var verificationResult = await provider.VerifyCodeAsync(challengeData.UserId, code, ct);
            if (verificationResult.IsFailure)
                return verificationResult;

            await _twoFactorChallengeStore.RemoveAsync(challengeId);
            return verificationResult.Value;
        }

        public async Task<Result<SetupAuthenticatorResult>> SetupAuthenticatorAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null)
                return Error.NotFound("user.not_found");

            var isAuthenticatorEnabled = user.TwoFactorEnabled && user.PreferredTwoFactorProvider == TwoFactorProvider.Authenticator;
            if (isAuthenticatorEnabled)
                return Error.Conflict("auth.2fa_already_enabled");

            await _userManager.ResetAuthenticatorKeyAsync(user);
            var key = await _userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrWhiteSpace(key))
                return Error.Failure("auth.2fa_authenticator_setup_failed");

            var authenticatorUri = $"otpauth://totp/MasarHub:{user.Email}?secret={key}&issuer=MasarHub";
            return new SetupAuthenticatorResult(key, authenticatorUri);
        }

        public async Task<Result<EnableTwoFactorResult>> VerifyAuthenticatorSetupAsync(Guid userId, string code)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return Error.NotFound("user.not_found");

            var verificationResult = await _userManager.VerifyTwoFactorTokenAsync(user, TokenOptions.DefaultAuthenticatorProvider, code);
            if (!verificationResult)
                return Error.BadRequest("auth.2fa_verification_failed");

            user.EnableTwoFactor(TwoFactorProvider.Authenticator);

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return Error.Failure("auth.2fa_enable_failed");

            return new EnableTwoFactorResult(userId, user.FullName, user.Email!, TwoFactorProvider.Authenticator);
        }

        public async Task<Result<IEnumerable<string>>> GenerateRecoveryCodesAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
                return Error.NotFound("user.not_found");

            if (!user.TwoFactorEnabled)
                return Error.BadRequest("auth.2fa_not_enabled");

            var codes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
            return Result<IEnumerable<string>>.Success(codes!);
        }
    }
}
