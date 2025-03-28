using Microsoft.EntityFrameworkCore;
using Store_API.Data;
using Store_API.DTOs.Stocks;
using Store_API.IRepositories;
using Store_API.Models.Inventory;

namespace Store_API.Repositories
{
    public class StockRepository : Repository<Stock>, IStockRepository
    {
        public StockRepository(StoreContext db, IDapperService dapperService) : base(db, dapperService)
        {

        }

        #region Retrieve Data

        public async Task<IEnumerable<StockDTO>> GetStocks()
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
	                            , s.Quantity
	                            , s.Updated

                            FROM Stocks s
                            INNER JOIN ProductDetails detail ON s.ProductDetailId = detail.Id
                            INNER JOIN Products product ON product.Id = detail.ProductId
                            INNER JOIN Warehouses wh ON wh.Id = s.WarehouseId
                            ";

            var stocksDTO = await _dapperService.QueryAsync<StockDTO>(query, null);
            if (stocksDTO == null || stocksDTO.Count == 0) return null;
            return stocksDTO;
        }

        public async Task<StockDTO> GetStock(Guid productDetailId)
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
	                            , s.Quantity
	                            , s.Updated

                            FROM Stocks s
                            INNER JOIN ProductDetails detail ON s.ProductDetailId = detail.Id
                            INNER JOIN Products product ON product.Id = detail.ProductId
                            INNER JOIN Warehouses wh ON wh.Id = s.WarehouseId

                            WHERE detail.Id = @Id
                            ";

            var stockDTO = await _dapperService.QueryFirstOrDefaultAsync<StockDTO>(query, new { Id = productDetailId });
            if (stockDTO == null) return null;
            return stockDTO;
        }

        #endregion


        #region CRUD Operations
        public override async Task AddAsync(Stock entity)
        {
            var stock = await _db.Stocks.FirstOrDefaultAsync(s => s.Id == entity.Id);
            if(stock != null)
            {
                stock.Quantity += entity.Quantity;
            }
            else
            {
                await base.AddAsync(entity);
            }
        }

        public override async void UpdateAsync(Stock entity)
        {
            var stock = await _db.Stocks.FirstOrDefaultAsync(s => s.Id == entity.Id);
            if (stock == null) return;

            stock.Quantity = entity.Quantity;
            _db.Update(stock);
        }

        #endregion
    }
}
