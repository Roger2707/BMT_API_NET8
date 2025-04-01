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
 
                            WHERE s.ProductDetailId = @ProductDetailId
                            ";

            var result = await _dapperService.QueryAsync<StockDapperRow>(query, new { ProductDetailId  = productDetailId});
            if (result == null || result.Count == 0) return null;

            var stockDTO = result
                .GroupBy(g => new {g.ProductDetailId, g.ProductName, g.Color, g.Price})
                .Select(s => new StockDTO
                {
                    ProductDetailId = s.Key.ProductDetailId,
                    ProductName = s.Key.ProductName,
                    Color = s.Key.Color,
                    Price = s.Key.Price,
                    StockDetail = s.Select(d => new StockDetailDTO
                    {
                        StockId = d.ProductDetailId,
                        WarehouseId = d.WarehouseId,
                        WarehouseName = d.WarehouseName,
                        Quantity = d.Quantity,
                        Updated = d.Updated
                    }).ToList()
                }).FirstOrDefault();

            return stockDTO;
        }

        public async Task<StockWareHouseDTO> GetStock(Guid productDetailId, Guid wareHouseId)
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
 
                            WHERE s.ProductDetailId = @ProductDetailId AND s.WarehouseId = @WareHouseId
                            ";

            var result = await _dapperService.QueryFirstOrDefaultAsync<StockWareHouseDTO>(query, new { ProductDetailId = productDetailId, WarehouseId = wareHouseId });
            if (result == null) return null;
            return result;
        }

        #endregion
    }
}
