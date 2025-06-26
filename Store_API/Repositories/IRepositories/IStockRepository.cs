using Store_API.DTOs.Stocks;
using Store_API.Models.Inventory;
using Store_API.Repositories;

namespace Store_API.Repositories.IRepositories
{
    public interface IStockRepository : IRepository<Stock>
    {
        Task<StockExistDTO> GetStockExisted(Guid productDetailId);
        Task<StockDTO> GetStock(Guid productDetailId);
        Task<StockQuantity> CheckExistedStock(Guid productDetailId, Guid warehouseId);
    }
}
