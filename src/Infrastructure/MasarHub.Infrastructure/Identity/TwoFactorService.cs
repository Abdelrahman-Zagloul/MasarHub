using MasarHub.Application.Abstractions.Identity;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Application.Features.Authentication.Commands.TwoFactor.EnableTwoFactor;
using MasarHub.Domain.Modules.Profiles;
using MasarHub.Infrastructure.Persistence.Identity;
using Microsoft.AspNetCore.Identity;

namespace MasarHub.Infrastructure.Identity
{
    public sealed class TwoFactorService : ITwoFactorService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public TwoFactorService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Result<EnableTwoFactorResult>> EnableAsync(Guid userId, TwoFactorProvider provider)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return Error.NotFound("user.not_found");

            if (user.TwoFactorEnabled)
                return Error.Conflict("auth.2fa_already_enabled");

            user.EnableTwoFactor(provider);

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return Error.Failure("auth.2fa_enable_failed");

            return new EnableTwoFactorResult(user.Id, user.FullName, user.Email!, user.PreferredTwoFactorProvider!.Value);
        }
    }
}
