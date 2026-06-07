using CloudinaryDotNet;
using MasarHub.Application.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace MasarHub.Infrastructure.Extensions
{
    public static class CloudinaryExtensions
    {
        public static IServiceCollection AddCloudinary(this IServiceCollection services)
        {
            services.AddSingleton(sp =>
            {
                var settings = sp.GetRequiredService<IOptions<CloudinarySettings>>().Value;

                var account = new Account(settings.CloudName, settings.APIKey, settings.APISecret);
                var cloudinary = new Cloudinary(account)
                {
                    Api = { Secure = true }
                };

                return cloudinary;
            });

            return services;
        }
    }
}
