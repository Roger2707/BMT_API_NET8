using Store_API.Data;
using Store_API.Infrastructures;
using Store_API.Models.OrderAggregate;
using Store_API.Repositories.IRepositories;

namespace Store_API.Repositories
{
    public class ShippingOrderRepository : Repository<ShippingOrder>, IShippingOrderRepository
    {
        public ShippingOrderRepository(StoreContext db, IDapperService dapperService) : base(db, dapperService)
        {
        }
    }
}
