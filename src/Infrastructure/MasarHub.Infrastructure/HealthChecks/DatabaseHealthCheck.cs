using MasarHub.Infrastructure.Persistence.Contexts;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace MasarHub.Infrastructure.HealthChecks
{
    public sealed class DatabaseHealthCheck : IHealthCheck
    {
        private readonly MasarHubDbContext _dbContext;

        public DatabaseHealthCheck(MasarHubDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var canConnect = await _dbContext.Database.CanConnectAsync(cancellationToken);

                return canConnect ?
                    HealthCheckResult.Healthy("Database is reachable") :
                    HealthCheckResult.Unhealthy("Cannot connect to database");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("Database health check failed", ex);
            }
        }
    }
}