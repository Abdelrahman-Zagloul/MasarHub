using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;

namespace MasarHub.Infrastructure.HealthChecks
{
    public sealed class RedisHealthCheck : IHealthCheck
    {
        private readonly IConnectionMultiplexer _redis;

        public RedisHealthCheck(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var pong = await _redis.GetDatabase().PingAsync();
                return HealthCheckResult.Healthy($"Redis responded in {pong.TotalMilliseconds} ms");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("Redis is unavailable", ex);
            }
        }
    }
}
