using Store_API.DTOs.Orders;

namespace Store_API.IService
{
    public interface IOrderService
    {
        Task Create(OrderCreateRequest orderCreateRequest);
        Task<IEnumerable<OrderDTO>> GetOrders(int userId);
    }
}
