using Store_API.DTOs.Stocks;
using Store_API.IService;
using Store_API.Models.Inventory;
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

        public async Task<StockDTO> GetStock(Guid productDetailId)
        {
            var stock = await _unitOfWork.Stock.GetStock(productDetailId);
            return stock;
        }

        public async Task<StockWarehouseDTO> GetStock(Guid productDetailId, Guid warehouseId)
        {
            var stock = await _unitOfWork.Stock.GetStock(productDetailId, warehouseId);
            return stock;
        }

        #endregion

        #region Retrieve Stock Transactions

        public async Task<int> GetCurrentQuantityInStock(Guid productDetailId)
        {
            int quantity = await _unitOfWork.StockTransaction.GetCurrentQuantityInStock(productDetailId);
            return quantity;
        }

        public async Task<IEnumerable<StockTransactionDTO>> GetProductDetailStockTransactions(Guid productId)
        {
            var stockTransactions = await _unitOfWork.StockTransaction.GetStockTransactions(productId);
            return stockTransactions;
        }

        #endregion

        #region Import / Export Stock

        public async Task<bool> ImportStock(StockUpsertDTO stockUpsertDTO)
        {
            var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                // Add New Stock Transaction
                var stockTransaction = new StockTransaction
                {
                    Id = Guid.NewGuid(),
                    ProductDetailId = stockUpsertDTO.ProductDetailId,
                    WarehouseId = stockUpsertDTO.WarehouseId,
                    Quantity = stockUpsertDTO.Quantity,
                    TransactionType = 1,
                    Created = DateTime.UtcNow,
                };
                await _unitOfWork.StockTransaction.AddAsync(stockTransaction);

                // Handle Stock
                var existedStock = await _unitOfWork.Stock.GetByIdAsync(stockUpsertDTO.StockId);
                if (existedStock != null)
                    existedStock.Quantity += stockUpsertDTO.Quantity;
                else
                {
                    var stock = new Stock
                    {
                        Id = Guid.NewGuid(),
                        ProductDetailId = stockUpsertDTO.ProductDetailId,
                        WarehouseId = stockUpsertDTO.WarehouseId,
                        Quantity = stockUpsertDTO.Quantity,
                        Updated = DateTime.UtcNow,
                    };
                    await _unitOfWork.Stock.AddAsync(stock);
                }

                await _unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch(Exception ex)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> ExportStock(StockUpsertDTO stockUpsertDTO)
        {
            var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                // Handle Stock
                var existedStock = await _unitOfWork.Stock.GetStock(stockUpsertDTO.ProductDetailId, stockUpsertDTO.WarehouseId);

                if (existedStock == null || existedStock.Quantity < stockUpsertDTO.Quantity)
                    throw new Exception("Stock is not enough !");

                //existedStock.Quantity -= stockUpsertDTO.Quantity;

                var updatedStock = await _unitOfWork.Stock.GetByIdAsync(existedStock.StockId);
                updatedStock.Quantity -= stockUpsertDTO.Quantity;

                // Add new Stock Transaction
                var stockTransaction = new StockTransaction
                {
                    Id = Guid.NewGuid(),
                    ProductDetailId = stockUpsertDTO.ProductDetailId,
                    WarehouseId = stockUpsertDTO.WarehouseId,
                    Quantity = stockUpsertDTO.Quantity,
                    TransactionType = 0,
                    Created = DateTime.UtcNow,
                };
                await _unitOfWork.StockTransaction.AddAsync(stockTransaction);

                await _unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch(Exception ex)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        #endregion
    }
}
