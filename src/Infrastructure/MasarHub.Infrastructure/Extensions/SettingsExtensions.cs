using MasarHub.Application.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MasarHub.Infrastructure.Extensions
{
    public static class SettingsExtensions
    {
        public static IServiceCollection AddInfrastructureSettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<LocalizationSettings>(configuration.GetSection(nameof(LocalizationSettings)));
            services.Configure<JWTSettings>(configuration.GetSection(nameof(JWTSettings)));
            services.Configure<DefaultUsersSettings>(configuration.GetSection(nameof(DefaultUsersSettings)));

            return services;
        }
    }
}
