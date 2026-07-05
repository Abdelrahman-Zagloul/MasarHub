using FluentValidation;
using MasarHub.Application.Common.Behaviors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace MasarHub.Application.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, string? mediatRLicenseKey)
        {
            services.AddMediatR(cfg =>
            {
                cfg.LicenseKey = mediatRLicenseKey;
                cfg.RegisterServicesFromAssembly(typeof(IApplicationAssemblyMarker).Assembly);
            });

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            services.AddValidatorsFromAssembly(typeof(IApplicationAssemblyMarker).Assembly);
            return services;
        }
    }
}