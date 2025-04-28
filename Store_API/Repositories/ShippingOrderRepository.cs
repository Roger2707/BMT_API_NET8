using Store_API.Data;
using Store_API.IRepositories;
using Store_API.IService;
using Store_API.Models.OrderAggregate;

namespace Store_API.Repositories
{
    public class ShippingOrderRepository : Repository<ShippingOrder>, IShippingOrderRepository
    {
        public ShippingOrderRepository(StoreContext db, IDapperService dapperService) : base(db, dapperService)
        {
        }
    }
}
