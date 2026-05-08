using MasarHub.API.SignalR.Hubs;
using MasarHub.API.SignalR.Services;
using MasarHub.Application.Abstractions.Services;

namespace MasarHub.API.Extensions
{
    public static class SignalRExtensions
    {
        public static IServiceCollection AddSignalRServices(this IServiceCollection services)
        {
            services.AddSignalR();
            services.AddScoped<INotificationRealtimeService, NotificationRealtimeService>();
            return services;
        }
        public static IEndpointRouteBuilder MapSignalRHubs(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapHub<NotificationHub>("/hubs/notifications");
            return endpoints;
        }
    }
}
