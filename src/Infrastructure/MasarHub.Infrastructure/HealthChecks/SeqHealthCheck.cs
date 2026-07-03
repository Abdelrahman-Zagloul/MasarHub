using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace MasarHub.Infrastructure.HealthChecks
{
    public sealed class SeqHealthCheck : IHealthCheck
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public SeqHealthCheck(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var serverUrl = GetSeqServerUrl();
            if (string.IsNullOrWhiteSpace(serverUrl))
                return HealthCheckResult.Degraded("Seq serverUrl is not configured.");

            try
            {
                var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync(serverUrl, cancellationToken);

                return response.IsSuccessStatusCode
                    ? HealthCheckResult.Healthy("Seq is reachable.")
                    : HealthCheckResult.Degraded($"Seq returned {(int)response.StatusCode}.");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Degraded("Seq is not reachable.", ex);
            }
        }

        private string? GetSeqServerUrl()
        {
            return _configuration.GetSection("Serilog:WriteTo")
                .GetChildren()
                .FirstOrDefault(x => string.Equals(x["Name"], "Seq", StringComparison.OrdinalIgnoreCase))?["Args:serverUrl"];
        }
    }
}
