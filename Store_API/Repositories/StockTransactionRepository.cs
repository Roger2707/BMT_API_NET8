using Store_API.Data;
using Store_API.DTOs.Stocks;
using Store_API.Helpers;
using Store_API.Infrastructures;
using Store_API.Models.Inventory;
using Store_API.Repositories.IRepositories;

namespace Store_API.Repositories
{
    public class StockTransactionRepository : Repository<StockTransaction>, IStockTransactionRepository
    {
        public StockTransactionRepository(StoreContext db, IDapperService dapperService) : base(db, dapperService)
        {

        }

        public async Task<IEnumerable<StockTransactionDTO>> GetStockTransactions(Guid productDetailId)
        {
            string query = @"
                            SELECT
                                s.Id

                                , s.ProductDetailId
                                , product.Name as ProductName
                                , detail.Price
                                , detail.Color

                                , s.WarehouseId
                                , wh.Name as WarehouseName

	                            , IIF(s.TransactionType = 1, 'Import', 'Export') as TransactionType
                                , s.Quantity
                                , s.Created

                            FROM StockTransactions s
                            INNER JOIN ProductDetails detail ON s.ProductDetailId = detail.Id
                            INNER JOIN Products product ON product.Id = detail.ProductId
                            INNER JOIN Warehouses wh ON wh.Id = s.WarehouseId

                            WHERE detail.Id = @productDetailId
                            ";

            var stockTransactions = await _dapperService.QueryAsync<StockTransactionDTO>(query, new { ProductDetailId = productDetailId });
            return stockTransactions;
        }
    }
}
