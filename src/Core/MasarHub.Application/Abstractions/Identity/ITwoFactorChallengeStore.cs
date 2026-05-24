using MasarHub.Application.Common.DependencyInjection;
using MasarHub.Domain.Modules.Profiles;

namespace MasarHub.Application.Abstractions.Identity
{
    public interface ITwoFactorChallengeStore : IScopedService
    {
        Task<Guid> CreateAsync(Guid userId, TwoFactorProvider provider, CancellationToken ct = default);

        Task<TwoFactorChallengeData?> GetAsync(Guid challengeId, CancellationToken ct = default);

        Task RemoveAsync(Guid challengeId, CancellationToken ct = default);
    }
    public sealed record TwoFactorChallengeData(Guid UserId, TwoFactorProvider Provider);
}
