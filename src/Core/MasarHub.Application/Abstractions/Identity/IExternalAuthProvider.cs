using MasarHub.Application.Common.DependencyInjection;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Features.Authentication.Commands.Account.ExternalLogin;

namespace MasarHub.Application.Abstractions.Identity
{
    public interface IExternalAuthProvider : IScopedService
    {
        ExternalLoginProvider Provider { get; }
        Task<Result<ExternalUserInfo>> VerifyAsync(string token, CancellationToken cancellationToken = default);
    }
}
