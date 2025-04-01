using Store_API.DTOs.Stocks;

namespace Store_API.IService
{
    public interface IStockService
    {

        #region Retrieve Stock
        Task<StockDTO> GetStock(Guid productDetailId);
        Task<StockDTO> GetStock(Guid productDetailId, Guid wareHouseId);

        #endregion

        #region Retrieve Stock Transactions
        Task<int> GetCurrentQuantityInStock(Guid productDetailId);
        Task<IEnumerable<StockTransactionDTO>> GetProductDetailStockTransactions(Guid productId);

        #endregion

        #region Import / Export Stock

        Task<bool> ImportStock(StockUpsertDTO stockUpsertDTO);
        Task<bool> ExportStock(StockUpsertDTO stockUpsertDTO);

        #endregion
    }
}
