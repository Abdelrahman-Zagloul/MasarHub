using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MasarHub.Infrastructure.Persistence.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDatabase(configuration);
            services.AddIdentityServices();

            return services;
        }
    }
}
