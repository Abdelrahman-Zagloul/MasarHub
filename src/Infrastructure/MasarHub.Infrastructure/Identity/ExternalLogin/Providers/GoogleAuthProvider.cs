using Google.Apis.Auth;
using MasarHub.Application.Abstractions.Identity;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Application.Features.Authentication.Commands.Account.ExternalLogin;
using MasarHub.Application.Settings;
using Microsoft.Extensions.Options;

namespace MasarHub.Infrastructure.Identity.ExternalLogin.Providers
{
    public sealed class GoogleAuthProvider : IExternalAuthProvider
    {
        private readonly ExternalAuthSettings _settings;
        public ExternalLoginProvider Provider => ExternalLoginProvider.Google;
        public GoogleAuthProvider(IOptions<ExternalAuthSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task<Result<ExternalUserInfo>> VerifyAsync(string token, CancellationToken cancellationToken = default)
        {
            try
            {
                var validationSettings = new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = [_settings.Google.ClientId]
                };

                var payload = await GoogleJsonWebSignature.ValidateAsync(token, validationSettings);

                return new ExternalUserInfo
                (
                   payload.Email,
                   payload.Name,
                   payload.Subject,
                   ExternalLoginProvider.Google
               );
            }
            catch (Exception)
            {
                return Error.Unauthorized("auth.external_token_invalid");
            }
        }
    }
}
