using Store_API.Data;
using Store_API.DTOs.Baskets;
using Store_API.DTOs.Orders;
using Store_API.Models.OrderAggregate;

namespace Store_API.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IDapperService _dapperService;
        private readonly StoreContext _db;
        public OrderRepository(IDapperService dapperService, StoreContext db)
        {
            _dapperService = dapperService;
            _db = db;
        }

        public async Task Create(Order order)
        {
            await _db.Orders.AddAsync(order);
        }
    }
}
