using Store_API.DTOs.Stocks;

namespace Store_API.IService
{
    public interface IStockService
    {

        #region Retrieve Stock
        Task<IEnumerable<StockDTO>> GetStocks();
        Task<StockDTO> GetStock(Guid productDetailId);

        #endregion

        #region Retrieve Stock Transactions
        Task<int> GetCurrentQuantityInStock(Guid productDetailId);
        Task<IEnumerable<StockTransactionDTO>> GetStockTransactions();
        Task<IEnumerable<StockTransactionDTO>> GetProductDetailStockTransactions(Guid productId);
        Task<StockTransactionDTO> GetStockTransaction(Guid stockTransactionId);

        #endregion

        #region Import / Export Stock

        Task<bool> ImportStock(StockUpsertDTO stockUpsertDTO);
        Task<bool> ExportStock(StockUpsertDTO stockUpsertDTO);

        #endregion
    }
}
