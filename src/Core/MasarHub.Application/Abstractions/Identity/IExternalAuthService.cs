using MasarHub.Application.Common.DependencyInjection;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Features.Authentication.Commands.Account.ExternalLogin;

namespace MasarHub.Application.Abstractions.Identity
{
    public interface IExternalAuthService : IScopedService
    {
        Task<Result<ExternalLoginResult>> LoginAsync(ExternalUserInfo userInfo);
    }
}
