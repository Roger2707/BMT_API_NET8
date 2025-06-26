using Store_API.DTOs.Orders;

namespace Store_API.Services.IService
{
    public interface IOrderService
    {
        Task Create(OrderCreateRequest orderCreateRequest);
        Task UpdateOrderStatus(OrderUpdatStatusRequest request);
        Task<IEnumerable<OrderDTO>> GetOrders();
        Task<IEnumerable<OrderDTO>> GetOrders(int userId);
    }
}
