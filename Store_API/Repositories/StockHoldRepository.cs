using Store_API.Data;
using Store_API.Infrastructures;
using Store_API.Models.Inventory;
using Store_API.Repositories.IRepositories;

namespace Store_API.Repositories
{
    public class StockHoldRepository : Repository<StockHold>, IStockHoldRepository
    {
        public StockHoldRepository(StoreContext db, IDapperService dapperService) : base(db, dapperService)
        {

        }
    }
}
