using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace MasarHub.Infrastructure.Extensions
{
    public static class RedisExtensions
    {
        public static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("Redis")
                ?? throw new InvalidOperationException("Redis connection string 'Redis' is missing.");

            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var logger = sp.GetRequiredService<ILoggerFactory>().CreateLogger("Redis");


                var options = ConfigurationOptions.Parse(connectionString, true);
                options.AbortOnConnectFail = false;
                options.ConnectRetry = 5;
                options.ConnectTimeout = 5000;
                options.SyncTimeout = 5000;
                options.KeepAlive = 60;
                options.ClientName = "MasarHub";


                var multiplexer = ConnectionMultiplexer.Connect(options);

                multiplexer.ConnectionFailed += (_, e) =>
                    logger.LogError("Redis connection failed: {EndPoint}", e.EndPoint);

                multiplexer.ConnectionRestored += (_, e) =>
                    logger.LogInformation("Redis connection restored: {EndPoint}", e.EndPoint);

                multiplexer.ErrorMessage += (_, e) =>
                    logger.LogError("Redis error: {Message}", e.Message);

                return multiplexer;
            });

            return services;
        }
    }
}