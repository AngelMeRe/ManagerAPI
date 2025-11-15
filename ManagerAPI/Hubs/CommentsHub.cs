using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ManagerAPI.Hubs
{
    // Hub de SignalR para manejar comentarios
    [Authorize]
    public class CommentsHub : Hub
    {
        public Task AddToGroup(string group) => Groups.AddToGroupAsync(Context.ConnectionId, group);
        public Task RemoveFromGroup(string group) => Groups.RemoveFromGroupAsync(Context.ConnectionId, group);
    }
}
