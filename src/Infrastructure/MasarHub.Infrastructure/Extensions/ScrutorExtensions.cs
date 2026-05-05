using MasarHub.Application.Common.DI;
using MasarHub.Infrastructure;
using MasarHub.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace MasarHub.Application.Extensions
{
    public static class ScrutorExtensions
    {
        public static IServiceCollection AddScrutor(this IServiceCollection services)
        {
            services.Scan(scan => scan
                .FromAssemblies([
                    typeof(IApplicationAssemblyMarker).Assembly,
                    typeof(IInfrastructureAssemblyMarker).Assembly,
                    typeof(IPersistenceAssemblyMarker).Assembly
                ])

                .AddClasses(c => c.AssignableTo<IScopedService>().Where(t => !t.IsAbstract))
                    .AsImplementedInterfaces()
                    .WithScopedLifetime()

                .AddClasses(c => c.AssignableTo<ITransientService>().Where(t => !t.IsAbstract))
                    .AsImplementedInterfaces()
                    .WithTransientLifetime()

                .AddClasses(c => c.AssignableTo<ISingletonService>().Where(t => !t.IsAbstract))
                    .AsImplementedInterfaces()
                    .WithSingletonLifetime()
            );

            return services;
        }
    }
}