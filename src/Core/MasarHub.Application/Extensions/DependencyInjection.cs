using Microsoft.Extensions.DependencyInjection;

namespace MasarHub.Application.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(IApplicationAssemblyMarker).Assembly));

            return services;
        }
    }
}
