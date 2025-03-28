using Store_API.DTOs.Stocks;
using Store_API.IService;
using Store_API.Repositories;

namespace Store_API.Services
{
    public class StockService : IStockService
    {
        private readonly IUnitOfWork _unitOfWork;

        public StockService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region Retrieve Stocks

        public async Task<IEnumerable<StockDTO>> GetStocks()
        {
            var stocks = await _unitOfWork.Stock.GetStocks();
            return stocks.ToList();
        }

        public async Task<StockDTO> GetStock(Guid productDetailId)
        {
            var stock = await _unitOfWork.Stock.GetStock(productDetailId);
            return stock;
        }

        #endregion

        #region Retrieve Stock Transactions

        public async Task<IEnumerable<StockTransactionDTO>> GetStockTransactions()
        {
            var stockTransactions = await _unitOfWork.StockTransaction.GetAllStockTransactions();
            return stockTransactions;
        }

        public async Task<StockTransactionDTO> GetStockTransaction(Guid stockTransactionId)
        {
            var stockTransaction = await _unitOfWork.StockTransaction.GetStockTransaction(stockTransactionId);
            return stockTransaction;
        }

        public async Task<IEnumerable<StockTransactionDTO>> GetProductDetailStockTransactions(Guid productId)
        {
            var stockTransactions = await _unitOfWork.StockTransaction.GetStockTransactions(productId);
            return stockTransactions;
        }

        #endregion
    }
}
