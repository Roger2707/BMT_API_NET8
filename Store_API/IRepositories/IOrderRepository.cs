using Store_API.DTOs.Orders;
using Store_API.Models.OrderAggregate;

namespace Store_API.Repositories
{
    public interface IOrderRepository
    {
        Task Create(Order order);
        Task<OrderDTO> GetOrder(Guid orderId);
        Task<IEnumerable<OrderItemDapperRow>> GetOrder(string clientSecret);
        Task<IEnumerable<OrderDTO>> GetOrders(int userId);
        Task<Order> FirstOrDefaultAsync(Guid orderId);
    }
}
