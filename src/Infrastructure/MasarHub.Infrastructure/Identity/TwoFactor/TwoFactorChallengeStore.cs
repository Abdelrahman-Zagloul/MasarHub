using MasarHub.Application.Abstractions.ExternalServices;
using MasarHub.Application.Abstractions.Identity;
using MasarHub.Domain.Modules.Profiles;

namespace MasarHub.Infrastructure.Identity.TwoFactor
{
    public sealed class TwoFactorChallengeStore : ITwoFactorChallengeStore
    {
        private readonly ICacheService _cacheService;
        private static string GetKey(Guid challengeId) => $"2fa:challenge:{challengeId}";

        public TwoFactorChallengeStore(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }

        public async Task<Guid> CreateAsync(Guid userId, TwoFactorProvider provider, CancellationToken ct = default)
        {
            var challengeId = Guid.CreateVersion7();

            var data = new TwoFactorChallengeData(userId, provider);
            await _cacheService.SetAsync(GetKey(challengeId), data, TimeSpan.FromMinutes(5), ct);
            return challengeId;
        }

        public async Task<TwoFactorChallengeData?> GetAsync(Guid challengeId, CancellationToken ct = default)
        {
            return await _cacheService.GetAsync<TwoFactorChallengeData>(GetKey(challengeId), ct);
        }

        public async Task RemoveAsync(Guid challengeId, CancellationToken ct = default)
        {
            await _cacheService.RemoveAsync(GetKey(challengeId), ct);
        }

    }
}