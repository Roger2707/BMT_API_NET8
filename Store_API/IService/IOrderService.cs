using Store_API.DTOs.Baskets;
using Store_API.DTOs.Orders;
using Store_API.Models.OrderAggregate;

namespace Store_API.IService
{
    public interface IOrderService
    {
        Task<Order> Create(int userId, UserAddressDTO address, BasketDTO basket);
    }
}
