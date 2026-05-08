using MasarHub.API.SignalR.Hubs;
using MasarHub.Application.Abstractions.Services;
using Microsoft.AspNetCore.SignalR;

namespace MasarHub.API.SignalR.Services
{
    public class NotificationRealtimeService : INotificationRealtimeService
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationRealtimeService(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendToAdminsAsync(object data, CancellationToken cancellationToken = default)
        {
            await _hubContext.Clients
                .Group(SignalRGroups.Admins)
                .SendAsync("NotificationReceived", data, cancellationToken);
        }
    }
}
