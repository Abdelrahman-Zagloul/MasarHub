using MasarHub.API.Middlewares;
using System.Text.Json.Serialization;

namespace MasarHub.API.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddAPI(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers()
                .AddJsonOptions(cfg =>
                {
                    cfg.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });


            services
                .AddOpenApi()
                .AddTransient<CultureMiddleware>()
                .AddJwtAuthentication(configuration)
                .AddCorsPolicy(configuration)
                .AddApiDocumentation()
                .AddSignalRServices()
                .AddProblemDetails()
                .AddVersioning()
                .AddRateLimitingConfiguration()
                .AddProxyConfiguration()
                .AddAppHealthChecks(configuration);


            return services;

        }
    }
}
