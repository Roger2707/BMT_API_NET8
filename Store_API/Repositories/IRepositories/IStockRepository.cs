using Store_API.DTOs.Stocks;
using Store_API.Models.Inventory;
using Store_API.Repositories;

namespace Store_API.Repositories.IRepositories
{
    public interface IStockRepository : IRepository<Stock>
    {
        Task<StockAvailable> GetAvailableStock(Guid productDetailId, int requiredQuantity);
        Task<StockDTO> GetStock(Guid productDetailId);
        Task<StockQuantity> CheckStockQuantityInWarehouse(Guid productDetailId, Guid warehouseId);
    }
}
