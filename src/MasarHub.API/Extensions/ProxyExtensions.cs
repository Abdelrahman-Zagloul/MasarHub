using Microsoft.AspNetCore.HttpOverrides;

namespace MasarHub.API.Extensions
{
    public static class ProxyExtensions
    {
        public static IServiceCollection AddProxyConfiguration(this IServiceCollection services)
        {
            // Required to get the real client IP address when using reverse proxies 
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });

            return services;
        }
    }
}
