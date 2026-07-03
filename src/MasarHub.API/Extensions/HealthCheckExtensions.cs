using MasarHub.Infrastructure.HealthChecks;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text.Json;

namespace MasarHub.API.Extensions
{
    public static class HealthCheckExtensions
    {
        public static IServiceCollection AddAppHealthChecks(this IServiceCollection services, IConfiguration configuration)
        {
            var healthChecks = services.AddHealthChecks();

            healthChecks.AddCheck("Application", () => HealthCheckResult.Healthy("Application is running."));

            healthChecks.AddCheck<DatabaseHealthCheck>
            (
                name: "Database",
                failureStatus: HealthStatus.Unhealthy,
                tags: ["ready", "critical", "sql"]
            );

            healthChecks.AddCheck<RedisHealthCheck>
            (
                name: "Redis",
                failureStatus: HealthStatus.Degraded,
                tags: ["cache", "redis", "optional"]
            );

            healthChecks.AddCheck<HangfireHealthCheck>
            (
                name: "Hangfire",
                failureStatus: HealthStatus.Degraded,
                tags: ["background jobs", "hangfire", "optional"]
            );

            healthChecks.AddCheck<SeqHealthCheck>
            (
                name: "Seq",
                failureStatus: HealthStatus.Degraded,
                tags: ["logging", "seq", "optional"]
            );

            return services;
        }

        public static WebApplication MapHealthChecks(this WebApplication app)
        {
            app.MapHealthChecks("/health", new HealthCheckOptions
            {
                ResponseWriter = async (context, report) =>
                {
                    context.Response.ContentType = "application/json";

                    var response = new
                    {
                        status = report.Status.ToString(),
                        totalDuration = report.TotalDuration.ToString(),
                        checks = report.Entries.Select(e => new
                        {
                            name = e.Key,
                            status = e.Value.Status.ToString(),
                            tags = e.Value.Tags,
                            description = e.Value.Description,
                            duration = e.Value.Duration.ToString(),
                            error = e.Value.Exception?.Message,
                            data = e.Value.Data
                        })
                    };

                    await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                }
            });

            app.MapHealthChecks("/health/live", new HealthCheckOptions
            {
                Predicate = _ => false
            });

            app.MapHealthChecks("/health/ready", new HealthCheckOptions
            {
                Predicate = check => check.Tags.Contains("ready")
            });

            return app;
        }
    }
}
