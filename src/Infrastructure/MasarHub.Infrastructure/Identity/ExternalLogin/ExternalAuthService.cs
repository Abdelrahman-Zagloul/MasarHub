using MasarHub.Application.Abstractions.Identity;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Features.Authentication.Commands.Account.ExternalLogin;
using MasarHub.Application.Features.Authentication.Shared;
using MasarHub.Infrastructure.Persistence.Identity;
using Microsoft.AspNetCore.Identity;

namespace MasarHub.Infrastructure.Identity.ExternalLogin
{
    public sealed class ExternalAuthService : IExternalAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ExternalAuthService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Result<ExternalLoginResult>> LoginAsync(ExternalUserInfo userInfo)
        {
            bool isNew = false;
            var provider = userInfo.Provider.ToString();
            var user = await _userManager.FindByLoginAsync(provider, userInfo.ProviderUserId);

            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(userInfo.Email);
                if (user == null)
                {
                    user = new ApplicationUser
                    {
                        UserName = userInfo.Email,
                        Email = userInfo.Email,
                        FullName = userInfo.FullName,
                        EmailConfirmed = true,
                        TwoFactorEnabled = false,
                    };

                    var createResult = await _userManager.CreateAsync(user);
                    if (!createResult.Succeeded)
                        return IdentityErrorsMapper.Map(createResult.Errors);

                    isNew = true;
                }

                var loginInfo = new UserLoginInfo(provider, userInfo.ProviderUserId, provider);
                var addLoginResult = await _userManager.AddLoginAsync(user, loginInfo);
                if (!addLoginResult.Succeeded)
                    return IdentityErrorsMapper.Map(addLoginResult.Errors);

            }


            var roles = await _userManager.GetRolesAsync(user);
            return new ExternalLoginResult(new TokenUser(user.Id, user.FullName, user.Email!, roles), user.FullName, isNew);
        }
    }
}
