using MasarHub.Application.Common.Constants;
using Microsoft.AspNetCore.SignalR;

namespace MasarHub.API.SignalR.Hubs
{
    public sealed class NotificationHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            if (Context.User?.IsInRole(Roles.Admin) == true)
                await Groups.AddToGroupAsync(Context.ConnectionId, SignalRGroups.Admins);

            await base.OnConnectedAsync();
        }
    }
}
