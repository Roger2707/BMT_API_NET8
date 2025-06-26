using Store_API.DTOs.Stocks;
using Store_API.Models.Inventory;
using Store_API.Repositories;

namespace Store_API.Repositories.IRepositories
{
    public interface IStockTransactionRepository : IRepository<StockTransaction>
    {
        Task<IEnumerable<StockTransactionDTO>> GetStockTransactions(Guid productDetailId);
    }
}
