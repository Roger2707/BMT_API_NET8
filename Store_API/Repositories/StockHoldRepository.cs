using Store_API.Data;
using Store_API.IRepositories;
using Store_API.IService;
using Store_API.Models.Inventory;

namespace Store_API.Repositories
{
    public class StockHoldRepository : Repository<StockHold>, IStockHoldRepository
    {
        public StockHoldRepository(StoreContext db, IDapperService dapperService) : base(db, dapperService)
        {

        }
    }
}
