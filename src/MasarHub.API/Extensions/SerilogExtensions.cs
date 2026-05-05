using Serilog;

namespace MasarHub.API.Extensions
{
    public static class SerilogExtensions
    {
        public static void AddSerilog(this WebApplicationBuilder builder)
        {
            builder.Logging.ClearProviders();

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateBootstrapLogger();

            builder.Host.UseSerilog((context, services, config) =>
            {
                config
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .Enrich.FromLogContext();
            });
        }
    }
}