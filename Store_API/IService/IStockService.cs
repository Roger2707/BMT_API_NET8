using Store_API.DTOs.Stocks;

namespace Store_API.IService
{
    public interface IStockService
    {

        #region Retrieve Stock
        Task<StockDTO> GetStock(Guid productDetailId);

        #endregion

        #region Retrieve Stock Transactions
        Task<IEnumerable<StockTransactionDTO>> GetStockTransactions(Guid productDetailId);

        #endregion

        #region Import / Export Stock

        Task<bool> ImportStock(StockUpsertDTO stockUpsertDTO);
        Task<bool> ExportStock(StockUpsertDTO stockUpsertDTO);

        #endregion
    }
}
