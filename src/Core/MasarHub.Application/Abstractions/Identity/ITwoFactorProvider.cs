using MasarHub.Application.Common.DependencyInjection;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Features.Authentication.Shared;
using MasarHub.Domain.Modules.Profiles;

namespace MasarHub.Application.Abstractions.Identity
{
    public interface ITwoFactorProvider : IScopedService
    {
        TwoFactorProvider Provider { get; }
        Task<Result> SendCodeAsync(Guid userId, CancellationToken ct = default);
        Task<Result<TokenUser>> VerifyCodeAsync(Guid userId, string code, CancellationToken ct = default);
    }
}
