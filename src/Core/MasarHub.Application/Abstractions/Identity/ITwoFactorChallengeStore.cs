using MasarHub.Application.Common.DI;
using MasarHub.Domain.Modules.Profiles;

namespace MasarHub.Application.Abstractions.Identity
{
    public interface ITwoFactorChallengeStore : IScopedService
    {
        Task<string> CreateAsync(Guid userId, TwoFactorProvider provider, CancellationToken ct = default);

        Task<TwoFactorChallengeData?> GetAsync(string challengeId, CancellationToken ct = default);

        Task RemoveAsync(string challengeId, CancellationToken ct = default);
    }
    public sealed record TwoFactorChallengeData(Guid UserId, TwoFactorProvider Provider);
}
