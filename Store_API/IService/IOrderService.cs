using Store_API.DTOs.Baskets;
using Store_API.DTOs.Orders;

namespace Store_API.IService
{
    public interface IOrderService
    {
        Task<OrderResponseDTO> Create(int userId, BasketDTO basket, int userAddressId, UserAddressDTO userAddressDTO);
        Task<OrderDTO> GetOrder(int orderId);
    }
}
