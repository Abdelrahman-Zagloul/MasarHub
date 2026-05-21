using MasarHub.API.Middlewares;

namespace MasarHub.API.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddAPI(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddTransient<CultureMiddleware>()
                .AddJwtAuthentication(configuration)
                .AddCorsPolicy(configuration)
                .AddSwaggerDocumentation()
                .AddSignalRServices()
                .AddProblemDetails()
                .AddVersioning();


            return services;

        }
    }
}
