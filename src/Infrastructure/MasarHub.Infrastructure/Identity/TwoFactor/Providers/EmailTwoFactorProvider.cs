using MasarHub.Application.Abstractions.ExternalServices;
using MasarHub.Application.Abstractions.Identity;
using MasarHub.Application.Abstractions.Services;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Application.Features.Authentication.Shared;
using MasarHub.Domain.Modules.Profiles;
using MasarHub.Infrastructure.Persistence.Identity;
using Microsoft.AspNetCore.Identity;

namespace MasarHub.Infrastructure.Identity.TwoFactor.Providers
{
    public sealed class EmailTwoFactorProvider : ITwoFactorProvider
    {
        private readonly IAppEmailService _appEmailService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IBackgroundJobService _backgroundJobService;
        public EmailTwoFactorProvider(IAppEmailService appEmailService, UserManager<ApplicationUser> userManager, IBackgroundJobService backgroundJobService)
        {
            _appEmailService = appEmailService;
            _userManager = userManager;
            _backgroundJobService = backgroundJobService;
        }

        public TwoFactorProvider Provider => TwoFactorProvider.Email;
        public async Task<Result> SendCodeAsync(Guid userId, CancellationToken ct = default)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return Error.NotFound("user.not_found");

            var code = await _userManager.GenerateTwoFactorTokenAsync(user, TokenOptions.DefaultEmailProvider);

            _backgroundJobService.Enqueue(() =>
                _appEmailService.SendTwoFactorCodeEmailAsync(user.FullName, user.Email!, code));

            return Result.Success();
        }

        public async Task<Result<TokenUser>> VerifyCodeAsync(Guid userId, string code, CancellationToken ct = default)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return Error.NotFound("user.not_found");

            var verificationResult = await _userManager.VerifyTwoFactorTokenAsync(user, TokenOptions.DefaultEmailProvider, code);
            if (!verificationResult)
                return Error.BadRequest("auth.2fa_verification_failed");

            var roles = await _userManager.GetRolesAsync(user);
            return new TokenUser(userId, user.Email!, roles);
        }
    }
}
