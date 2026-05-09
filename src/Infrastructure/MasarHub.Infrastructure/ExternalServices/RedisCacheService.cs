using MasarHub.Application.Abstractions.ExternalServices;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text.Json;

namespace MasarHub.Infrastructure.ExternalServices
{
    public sealed class RedisCacheService : ICacheService
    {
        private readonly IDatabase _database;
        private readonly ILogger<RedisCacheService> _logger;
        private static bool _redisUnavailable;
        private static readonly JsonSerializerOptions _options = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public RedisCacheService(IConnectionMultiplexer redis, ILogger<RedisCacheService> logger)
        {
            _logger = logger;
            _database = redis.GetDatabase();
            redis.ConnectionRestored += (_, _) =>
            {
                _redisUnavailable = false;
                _logger.LogInformation("Redis connection restored.");
            };
        }
        public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
        {
            try
            {
                var value = await _database.StringGetAsync(key);
                return value.IsNullOrEmpty ? default : JsonSerializer.Deserialize<T>(value.ToString(), _options);
            }
            catch (Exception ex)
            {
                LogRedisUnavailable(ex, key);
                return default;
            }
        }
        public async Task SetAsync<T>(string key, T value, TimeSpan expiration, CancellationToken ct = default)
        {
            try
            {
                await _database.StringSetAsync(key, JsonSerializer.Serialize(value, _options), expiration);
            }
            catch (Exception ex)
            {
                LogRedisUnavailable(ex, key);
            }
        }
        public async Task RemoveAsync(string key, CancellationToken ct = default)
        {
            try
            {
                await _database.KeyDeleteAsync(key);
            }
            catch (Exception ex)
            {
                LogRedisUnavailable(ex, key);
            }
        }
        private void LogRedisUnavailable(Exception ex, string key)
        {
            if (_redisUnavailable)
                return;

            _redisUnavailable = true;
            _logger.LogWarning(ex, "Redis cache is unavailable while processing key: {Key}", key);
        }

    }
}
