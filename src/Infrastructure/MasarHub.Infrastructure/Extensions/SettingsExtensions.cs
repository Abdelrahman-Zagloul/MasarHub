using MasarHub.Application.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MasarHub.Infrastructure.Extensions
{
    public static class SettingsExtensions
    {
        public static IServiceCollection AddInfrastructureSettings(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddAndValidateSettings<JWTSettings>(configuration, nameof(JWTSettings))
                .AddAndValidateSettings<LocalizationSettings>(configuration, nameof(LocalizationSettings))
                .AddAndValidateSettings<DefaultUsersSettings>(configuration, nameof(DefaultUsersSettings))
                .AddAndValidateSettings<MailSettings>(configuration, nameof(MailSettings))
                .AddAndValidateSettings<TwilioSettings>(configuration, nameof(TwilioSettings))
                .AddAndValidateSettings<FrontendURLsSettings>(configuration, nameof(FrontendURLsSettings))
                .AddAndValidateSettings<RefreshTokenSettings>(configuration, nameof(RefreshTokenSettings));



            return services;
        }
        public static IServiceCollection AddAndValidateSettings<T>(this IServiceCollection services, IConfiguration configuration, string sectionName) where T : class
        {
            services.AddOptions<T>()
                .Bind(configuration.GetSection(sectionName))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            return services;
        }
    }
}
