using Store_API.DTOs.Baskets;
using Store_API.DTOs.Orders;
using Store_API.Models.OrderAggregate;

namespace Store_API.IService
{
    public interface IOrderService
    {
        Task<OrderResponseDTO> Create(int userId, string userName, BasketDTO basket, int userAddressId);
        Task<OrderDTO> GetOrder(int orderId);
        Task<IEnumerable<OrderItemDapperRow>> GetOrder(string clientSecret);
        Task<IEnumerable<OrderDTO>> GetOrders(int userId);
        Task UpdateOrderStatus(int orderId, OrderStatus status);
    }
}
