using Store_API.DTOs.Baskets;
using Store_API.DTOs.Orders;

namespace Store_API.Repositories
{
    public interface IOrderRepository
    {
        //public Task<OrderDTO> GetAll(int userId);
        public Task Create(int userId, UserAddressDTO address, BasketDTO basket);
    }
}
