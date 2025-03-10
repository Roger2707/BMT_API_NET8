using Microsoft.AspNetCore.SignalR;

namespace Store_API.Hubs
{
    public class OrderHub : Hub
    {
        public async Task SendOrderUpdate(string orderId, int status)
        {
            await Clients.All.SendAsync("ReceiveOrderUpdate", orderId, status);
        }
    }
}

