using MasarHub.Application.Abstractions.Identity;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Application.Features.Authentication.Commands.Account.ExternalLogin;

namespace MasarHub.Infrastructure.Identity.ExternalLogin.Providers
{
    public sealed class FactbookAuthProvider : IExternalAuthProvider
    {
        public ExternalLoginProvider Provider => ExternalLoginProvider.Facebook;

        public async Task<Result<ExternalUserInfo>> VerifyAsync(string token, CancellationToken cancellationToken = default)
        {
            return Error.NotFound("auth.external_provider_coming_soon");
        }
    }
}
