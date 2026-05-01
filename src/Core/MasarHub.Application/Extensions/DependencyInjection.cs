using MasarHub.Application.Common.DI;
using Microsoft.Extensions.DependencyInjection;

namespace MasarHub.Application.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(IApplicationAssemblyMarker).Assembly));

            ConfigureScrutor(services);
            return services;
        }
        private static void ConfigureScrutor(IServiceCollection services)
        {

            services.Scan(scan => scan
                .FromAssemblies(typeof(IApplicationAssemblyMarker).Assembly)

                .AddClasses(c => c.AssignableTo<ITransientService>())
                    .AsMatchingInterface()
                    .WithTransientLifetime()

               .AddClasses(c => c.AssignableTo<IScopedService>())
                    .AsMatchingInterface()
                    .WithScopedLifetime()

                .AddClasses(c => c.AssignableTo<ISingletonService>())
                    .AsMatchingInterface()
                    .WithSingletonLifetime()
            );
        }
    }
}