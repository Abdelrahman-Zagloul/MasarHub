using MasarHub.Application.Abstractions.ExternalServices;
using MasarHub.Application.Abstractions.Identity;
using MasarHub.Domain.Modules.Profiles;

namespace MasarHub.Infrastructure.Identity
{
    public sealed class TwoFactorChallengeStore : ITwoFactorChallengeStore
    {
        private readonly ICacheService _cacheService;
        private static string GetKey(string challengeId) => $"2fa:challenge:{challengeId}";

        public TwoFactorChallengeStore(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }

        public async Task<string> CreateAsync(Guid userId, TwoFactorProvider provider, CancellationToken ct = default)
        {
            var challengeId = Guid.CreateVersion7().ToString();

            var data = new TwoFactorChallengeData(userId, provider);
            await _cacheService.SetAsync(GetKey(challengeId), data, TimeSpan.FromMinutes(5), ct);
            return challengeId;
        }

        public async Task<TwoFactorChallengeData?> GetAsync(string challengeId, CancellationToken ct = default)
        {
            return await _cacheService.GetAsync<TwoFactorChallengeData>(GetKey(challengeId), ct);
        }

        public async Task RemoveAsync(string challengeId, CancellationToken ct = default)
        {
            await _cacheService.RemoveAsync(GetKey(challengeId), ct);
        }

    }
}