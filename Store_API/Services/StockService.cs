using Store_API.DTOs.Stocks;
using Store_API.Enums;
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

        #endregion

        #region Retrieve Stock Transactions

        public async Task<IEnumerable<StockTransactionDTO>> GetStockTransactions(Guid productDetailId)
        {
            var stockTransactions = await _unitOfWork.StockTransaction.GetStockTransactions(productDetailId);
            return stockTransactions;
        }

        #endregion

        #region Import / Export Stock

        public async Task<bool> ImportStock(StockUpsertDTO stockUpsertDTO)
        {
            await _unitOfWork.BeginTransactionAsync(TransactionType.EntityFramework);
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
                    Created = DateTime.Now,
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
                        Updated = DateTime.Now,
                    };
                    await _unitOfWork.Stock.AddAsync(stock);
                }

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();
                return true;
            }
            catch(Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> ExportStock(StockUpsertDTO stockUpsertDTO)
        {
            await _unitOfWork.BeginTransactionAsync(TransactionType.EntityFramework);
            try
            {
                // Handle Stock
                var existedStock = await _unitOfWork.Stock.CheckExistedStock(stockUpsertDTO.ProductDetailId, stockUpsertDTO.WarehouseId);

                if (existedStock == null || existedStock.Quantity < stockUpsertDTO.Quantity)
                    throw new Exception("Quantity is not enough !");

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
                await _unitOfWork.CommitAsync();
                return true;
            }
            catch(Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        #endregion
    }
}
