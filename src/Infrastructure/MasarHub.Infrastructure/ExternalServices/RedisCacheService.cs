using MasarHub.Application.Abstractions.ExternalServices;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text.Json;

namespace MasarHub.Infrastructure.ExternalServices
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDatabase _database;
        private readonly IConnectionMultiplexer _redis;
        private readonly ILogger<RedisCacheService> _logger;
        private static readonly JsonSerializerOptions _options = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public RedisCacheService(IConnectionMultiplexer redis, ILogger<RedisCacheService> logger)
        {
            _redis = redis;
            _logger = logger;
            _database = redis.GetDatabase();
        }
        public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
        {
            try
            {
                var value = await _database.StringGetAsync(key);
                return value.IsNullOrEmpty ? default : JsonSerializer.Deserialize<T>(value.ToString(), _options);
            }
            catch
            {
                _logger.LogError("Error occurred while fetching cache value for key: {Key}", key);
                return default;
            }
        }
        public async Task SetAsync<T>(string key, T value, TimeSpan expiration, CancellationToken ct = default)
        {
            try
            {
                await _database.StringSetAsync(key, JsonSerializer.Serialize(value, _options), expiration);
            }
            catch
            {
                _logger.LogError("Error occurred while setting cache value for key: {Key}", key);
            }
        }
        public async Task RemoveAsync(string key, CancellationToken ct = default)
        {
            try
            {
                await _database.KeyDeleteAsync(key);
            }
            catch
            {
                _logger.LogError("Error occurred while removing cache value for key: {Key}", key);
            }
        }

    }
}
