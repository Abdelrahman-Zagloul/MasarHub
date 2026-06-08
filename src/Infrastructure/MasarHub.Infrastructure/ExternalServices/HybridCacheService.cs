using MasarHub.Application.Abstractions.ExternalServices;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MasarHub.Infrastructure.ExternalServices
{
    public sealed class HybridCacheService : ICacheService
    {
        private readonly ICacheService _redisCache;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<HybridCacheService> _logger;

        public HybridCacheService([FromKeyedServices("redis")] ICacheService redisCache, IMemoryCache memoryCache, ILogger<HybridCacheService> logger)
        {
            _redisCache = redisCache;
            _memoryCache = memoryCache;
            _logger = logger;
        }

        public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
        {
            if (_memoryCache.TryGetValue(key, out T? localValue) && localValue != null)
                return localValue;

            var redisValue = await _redisCache.GetAsync<T>(key, ct);
            if (redisValue != null)
                _memoryCache.Set(key, redisValue);

            return redisValue;
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan expiration, CancellationToken ct = default)
        {
            _memoryCache.Set(key, value, expiration);
            try
            {
                await _redisCache.SetAsync(key, value, expiration, ct);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "HybridCache: Failed to save to Redis for key {Key}, fallback to memory-only.", key);
            }
        }

        public async Task RemoveAsync(string key, CancellationToken ct = default)
        {
            _memoryCache.Remove(key);

            try
            {
                await _redisCache.RemoveAsync(key, ct);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "HybridCache: Failed to remove from Redis for key {Key}, removed from memory only.", key);
            }
        }
    }
}
