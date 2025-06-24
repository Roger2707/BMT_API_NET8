using Store_API.DTOs.Orders;

namespace Store_API.IService
{
    public interface IOrderService
    {
        Task Create(OrderCreateRequest orderCreateRequest);
        Task UpdateOrderStatus(OrderUpdatStatusRequest request, int userId);
        Task<IEnumerable<OrderDTO>> GetOrders();
        Task<IEnumerable<OrderDTO>> GetOrders(int userId);
    }
}
