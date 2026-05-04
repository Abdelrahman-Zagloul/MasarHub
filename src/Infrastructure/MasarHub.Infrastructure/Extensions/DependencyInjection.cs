using MasarHub.Application.Extensions;
using MasarHub.Infrastructure.Persistence.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MasarHub.Infrastructure.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services
            .AddDatabase(configuration)
            .AddIdentityServices()
            .AddRedis(configuration)
            .AddInfrastructureSettings(configuration)
            .ConfigureScrutor();

            services.AddHttpContextAccessor();
            return services;
        }
    }
}
