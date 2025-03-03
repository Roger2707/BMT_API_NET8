using Store_API.DTOs.Baskets;
using Store_API.DTOs.Orders;
using Store_API.Models.OrderAggregate;

namespace Store_API.IService
{
    public interface IOrderService
    {
        Task<int> Create(int userId, BasketDTO basket, int userAddressId, UserAddressDTO userAddressDTO);
        Task<OrderDTO> GetOrder(int orderId);
    }
}
