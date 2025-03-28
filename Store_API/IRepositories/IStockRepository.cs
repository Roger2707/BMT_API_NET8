using Store_API.DTOs.Stocks;
using Store_API.Models.Inventory;
using Store_API.Repositories;

namespace Store_API.IRepositories
{
    public interface IStockRepository : IRepository<Stock>
    {
        Task<IEnumerable<StockDTO>> GetStocks();
        Task<StockDTO> GetStock(Guid stockId);
    }
}
