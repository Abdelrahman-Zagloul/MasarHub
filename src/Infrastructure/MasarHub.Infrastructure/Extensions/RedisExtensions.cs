using Hangfire;
using MasarHub.Application.Abstractions.ExternalServices;
using MasarHub.Infrastructure.ExternalServices;
using MasarHub.Infrastructure.Services.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace MasarHub.Infrastructure.Extensions
{
    public static class RedisExtensions
    {
        private static bool _jobQueued;

        public static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("Redis")
                ?? throw new InvalidOperationException("Redis connection string 'Redis' is missing.");

            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var logger = sp.GetRequiredService<ILoggerFactory>().CreateLogger("Redis");


                var options = ConfigurationOptions.Parse(connectionString, true);
                options.AbortOnConnectFail = false;
                options.ConnectRetry = 1;
                options.ConnectTimeout = 1000;
                options.SyncTimeout = 1000;
                options.AsyncTimeout = 1000;
                options.KeepAlive = 60;
                options.ClientName = "MasarHub";

                var multiplexer = ConnectionMultiplexer.Connect(options);

                multiplexer.ConnectionFailed += (_, e) =>
                    logger.LogWarning("Redis connection failed: {EndPoint}", e.EndPoint);

                multiplexer.ConnectionRestored += (_, e) =>
                {
                    logger.LogInformation("Redis connection restored: {EndPoint}", e.EndPoint);

                    // Prevent duplicate enqueue
                    if (_jobQueued)
                        return;

                    _jobQueued = true;
                    BackgroundJob.Enqueue<LocalizationSyncJob>(x => x.SyncAsync(default));
                    Task.Delay(TimeSpan.FromSeconds(60)).ContinueWith(_ => _jobQueued = false);
                };

                multiplexer.ErrorMessage += (_, e) =>
                    logger.LogWarning("Redis error: {Message}", e.Message);

                return multiplexer;
            });

            services.AddMemoryCache();
            services.AddKeyedScoped<ICacheService, RedisCacheService>("redis");
            services.AddScoped<ICacheService, HybridCacheService>();
            return services;
        }
    }
}