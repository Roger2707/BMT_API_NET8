using Store_API.DTOs.Stocks;
using Store_API.Enums;
using Store_API.Models.Inventory;
using Store_API.Services.IService;
using Store_API.Infrastructures;

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

        #region Import / Export Stock - Admin - Transaction will be created now

        public async Task<bool> Import(StockUpsertDTO stockUpsertDTO)
        {
            await _unitOfWork.BeginTransactionAsync(TransactionType.EntityFramework);
            try
            {
                // Get stock quantity in warehouse - row level lock - consistency if multiple users import stock at the same time
                var stockQuantity = await _unitOfWork.Stock.CheckStockQuantityInWarehouse(stockUpsertDTO.ProductDetailId, stockUpsertDTO.WarehouseId);

                await ImportStock(stockUpsertDTO);

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }
       
        public async Task<bool> Export(StockUpsertDTO stockUpsertDTO)
        {
            await _unitOfWork.BeginTransactionAsync(TransactionType.EntityFramework);
            try
            {
                // Get stock quantity in warehouse - row level lock - consistency if multiple users export stock at the same time
                var stockQuantity = await _unitOfWork.Stock.CheckStockQuantityInWarehouse(stockUpsertDTO.ProductDetailId, stockUpsertDTO.WarehouseId);
                if (stockQuantity == null || stockQuantity.Quantity < stockUpsertDTO.Quantity)
                    throw new Exception("Quantity is not enough !");

                await ExportStock(stockUpsertDTO);

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

        #region Import / Export Stock - System Automation - Transaction will be created outside

        public async Task ImportStock(StockUpsertDTO stockUpsertDTO)
        {
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

            var existedStock = await _unitOfWork.Stock.FindFirstAsync(s => s.Id == stockUpsertDTO.StockId);
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
        }

        public async Task ExportStock(StockUpsertDTO stockUpsertDTO)
        {
            var updatedStock = await _unitOfWork.Stock.FindFirstAsync(s => s.Id == stockUpsertDTO.StockId);
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
        }

        #endregion

    }
}
