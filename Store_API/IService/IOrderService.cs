using Store_API.DTOs.Orders;
using Store_API.Enums;

namespace Store_API.IService
{
    public interface IOrderService
    {
        Task Create(OrderCreateRequest orderCreateRequest);
        Task UpdateOrderStatus(Guid orderId, OrderStatus status);
        Task<IEnumerable<OrderDTO>> GetOrders();
        Task<IEnumerable<OrderDTO>> GetOrders(int userId);
    }
}
