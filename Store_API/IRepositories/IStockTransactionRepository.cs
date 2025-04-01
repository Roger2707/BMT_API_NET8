using Store_API.DTOs.Stocks;
using Store_API.Models.Inventory;
using Store_API.Repositories;

namespace Store_API.IRepositories
{
    public interface IStockTransactionRepository : IRepository<StockTransaction>
    {
        Task<IEnumerable<StockTransactionDTO>> GetAllStockTransactions();
        Task<IEnumerable<StockTransactionDTO>> GetStockTransactions(Guid productDetailId);
        Task<StockTransactionDTO> GetStockTransaction(Guid stockId);
        Task<int> GetCurrentQuantityInStock(Guid productDetailId);
    }
}
