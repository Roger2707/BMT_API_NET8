using Store_API.DTOs.Baskets;
using Store_API.DTOs.Orders;
using Store_API.Models.OrderAggregate;

namespace Store_API.Repositories
{
    public interface IOrderRepository
    {
        //public Task<OrderDTO> GetAll(int userId);
        public Task<Order> Create(int userId, UserAddressDTO address, BasketDTO basket);
    }
}
