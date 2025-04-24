using Store_API.DTOs.Orders;

namespace Store_API.IService
{
    public interface IOrderService
    {
        Task<OrderResponseDTO> Create(OrderCreateRequest orderCreateRequest);
        Task<OrderDTO> GetOrder(int orderId);
        Task<IEnumerable<OrderItemDapperRow>> GetOrder(string clientSecret);
        Task<IEnumerable<OrderDTO>> GetOrders(int userId);
    }
}
