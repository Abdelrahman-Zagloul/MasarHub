using MasarHub.Application.Abstractions.ExternalServices;
using MasarHub.Application.Abstractions.Identity;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Domain.Modules.Profiles;
using MasarHub.Infrastructure.Persistence.Identity;
using Microsoft.AspNetCore.Identity;

namespace MasarHub.Infrastructure.Identity.TwoFactor.Providers
{
    public sealed class SmsTwoFactorProvider : ITwoFactorProvider
    {
        private readonly ISmsService _smsService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IBackgroundJobService _backgroundJobService;

        public SmsTwoFactorProvider(ISmsService smsService, UserManager<ApplicationUser> userManager, IBackgroundJobService backgroundJobService)
        {
            _smsService = smsService;
            _userManager = userManager;
            _backgroundJobService = backgroundJobService;
        }

        public TwoFactorProvider Provider => TwoFactorProvider.Sms;

        public async Task<Result> SendCodeAsync(Guid userId, CancellationToken ct = default)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null)
                return Error.NotFound("user.not_found");

            var code = await _userManager.GenerateTwoFactorTokenAsync(user, TokenOptions.DefaultPhoneProvider);

            _backgroundJobService.Enqueue(() =>
                _smsService.SendSmsAsync(user.PhoneNumber!, $"Your MasarHub verification code is: {code}"));

            return Result.Success();
        }
    }
}