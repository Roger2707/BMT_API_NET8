using Store_API.Data;
using Store_API.IRepositories;
using Store_API.Models.Inventory;

namespace Store_API.Repositories
{
    public class WarehouseRepository : Repository<Warehouse>, IWarehouseRepository
    {
        public WarehouseRepository(StoreContext db, IDapperService dapperService) : base(db, dapperService)
        {

        }
    }
}
