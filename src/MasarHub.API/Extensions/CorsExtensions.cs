using MasarHub.Application.Settings;

namespace MasarHub.API.Extensions
{
    public static class CorsExtensions
    {
        public static IServiceCollection AddCorsPolicy(this IServiceCollection services, IConfiguration configuration)
        {
            var settings = configuration.GetSection(nameof(CorsSettings)).Get<CorsSettings>();
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            services.AddCors(options =>
            {
                options.AddPolicy(CorsSettings.DevelopmentPolicy, policy =>
                {
                    policy
                        .SetIsOriginAllowed(_ => true)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });


                options.AddPolicy(CorsSettings.ProductionPolicy, policy =>
                {
                    policy
                        .WithOrigins(settings.AllowedOrigins)
                        .WithHeaders(settings.AllowedHeaders)
                        .WithMethods(settings.AllowedMethods);

                    if (settings.AllowCredentials)
                        policy.AllowCredentials();
                });
            });

            return services;
        }
    }
}

