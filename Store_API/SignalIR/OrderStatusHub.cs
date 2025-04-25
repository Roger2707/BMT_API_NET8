using Microsoft.AspNetCore.SignalR;

namespace Store_API.SignalIR
{
    public class OrderStatusHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }

        public async Task JoinGroup(string clientSecret)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, clientSecret);
        }

        public async Task LeaveGroup(string clientSecret)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, clientSecret);
        }
    }
}
