using MasarHub.Application.Common.DI;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Features.Authentication.Shared;

namespace MasarHub.Application.Abstractions.Identity
{
    public interface IRefreshTokenService : IScopedService
    {
        Task<Result<RefreshTokenResult>> CreateAsync(TokenUser user, string? ipAddress, CancellationToken ct = default);
        Task<Result<RefreshTokenResult>> RotateAsync(string refreshToken, string? ipAddress, CancellationToken ct = default);
        Task<Result> RevokeAsync(string refreshToken, string? ipAddress, CancellationToken ct = default);
        Task RevokeAllAsync(Guid userId, string? ipAddress, CancellationToken ct = default);
    }
}
