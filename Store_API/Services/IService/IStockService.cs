using Store_API.DTOs.Stocks;

namespace Store_API.Services.IService
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

        Task ImportStock(StockUpsertDTO stockUpsertDTO);
        Task<bool> Import(StockUpsertDTO stockUpsertDTO);
        Task ExportStock(StockUpsertDTO stockUpsertDTO);
        Task<bool> Export(StockUpsertDTO stockUpsertDTO);

        #endregion
    }
}
