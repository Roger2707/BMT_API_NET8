using Microsoft.AspNetCore.SignalR;
using Store_API.DTOs.Orders;

namespace Store_API.SignalIR
{
    public class OrderStatusHub : Hub
    {
        //public async Task SendOrderUpdateToAdmin(OrderUpdateHub message)
        //{
        //    await Clients.Group("Admins").SendAsync("ReceiveOrderUpdate", message);
        //}

        //public async Task SendOrderUpdateToUser(OrderUpdateHub message)
        //{
        //    await Clients.User(userId).SendAsync("ReceiveOrderUpdate", message);
        //}
    }
}
