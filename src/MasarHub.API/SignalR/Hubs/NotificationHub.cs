using MasarHub.Application.Common.Constants;
using Microsoft.AspNetCore.SignalR;

namespace MasarHub.API.SignalR.Hubs
{
    public sealed class NotificationHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            if (IsInRole(Roles.Admin))
                await Groups.AddToGroupAsync(Context.ConnectionId, SignalRGroups.Admins);

            if (IsInRole(Roles.Instructor))
                await Groups.AddToGroupAsync(Context.ConnectionId, SignalRGroups.Instructors);

            if (IsInRole(Roles.Student))
                await Groups.AddToGroupAsync(Context.ConnectionId, SignalRGroups.Students);

            await base.OnConnectedAsync();
        }
        private bool IsInRole(string role) => Context.User?.IsInRole(role) == true;
    }
}