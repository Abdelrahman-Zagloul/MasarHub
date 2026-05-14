using Hangfire;
using Hangfire.SqlServer;
using MasarHub.Infrastructure.Localization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MasarHub.Infrastructure.Extensions
{
    public static class HangfireExtensions
    {
        public static IServiceCollection AddHangfire(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddHangfire(options =>
            {
                options.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                        .UseSimpleAssemblyNameTypeSerializer()
                        .UseRecommendedSerializerSettings()
                        .UseSqlServerStorage(configuration.GetConnectionString("DefaultConnection"), new SqlServerStorageOptions
                        {
                            CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                            SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                            QueuePollInterval = TimeSpan.Zero,
                            UseRecommendedIsolationLevel = true,
                            DisableGlobalLocks = true
                        });
            });

            services.AddHangfireServer();

            services.AddScoped<LocalizationSyncJob>();
            return services;
        }

        public static IApplicationBuilder UseHangfireJobs(this IApplicationBuilder app)
        {
            BackgroundJob.Enqueue<LocalizationSyncJob>(x => x.SyncAsync(default));
            RecurringJob.AddOrUpdate<LocalizationSyncJob>("localization-cache-refresh", x => x.SyncAsync(default), Cron.Daily);

            return app;
        }
    }
}
