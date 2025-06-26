using Store_API.Data;
using Store_API.Models.Inventory;
using Store_API.Repositories.IRepositories;
using Store_API.Services.IService;

namespace Store_API.Repositories
{
    public class StockHoldRepository : Repository<StockHold>, IStockHoldRepository
    {
        public StockHoldRepository(StoreContext db, IDapperService dapperService) : base(db, dapperService)
        {

        }
    }
}
