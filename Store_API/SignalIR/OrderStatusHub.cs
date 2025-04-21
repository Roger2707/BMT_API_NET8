using Microsoft.AspNetCore.SignalR;

namespace Store_API.SignalIR
{
    public class OrderStatusHub : Hub
    {
        public async Task SendOrderUpdateToAdmin(string message)
        {
            await Clients.Group("Admins").SendAsync("ReceiveOrderUpdate", message);
        }

        public async Task SendOrderUpdateToUser(string userId, string message)
        {
            await Clients.User(userId).SendAsync("ReceiveOrderUpdate", message);
        }

        public async Task AddToAdminGroup(string userId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "Admins");
        }

        public async Task RemoveFromAdminGroup(string userId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "Admins");
        }
    }
}
