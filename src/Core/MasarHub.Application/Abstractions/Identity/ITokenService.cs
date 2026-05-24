using MasarHub.Application.Common.DependencyInjection;
using MasarHub.Application.Features.Authentication.Shared;

namespace MasarHub.Application.Abstractions.Identity
{
    public interface ITokenService : IScopedService
    {
        Task<AccessTokenResponse> GenerateTokenAsync(TokenUser user);
    }
}
