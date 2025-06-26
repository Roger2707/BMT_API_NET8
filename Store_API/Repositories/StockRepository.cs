using Store_API.Data;
using Store_API.DTOs.Stocks;
using Store_API.Models.Inventory;
using Store_API.Repositories.IRepositories;
using Store_API.Services.IService;

namespace Store_API.Repositories
{
    public class StockRepository : Repository<Stock>, IStockRepository
    {
        public StockRepository(StoreContext db, IDapperService dapperService) : base(db, dapperService)
        {

        }

        public async Task<StockQuantity> CheckExistedStock(Guid productDetailId, Guid warehouseId)
        {
            string query = @"SELECT Id as StockId, Quantity FROM Stocks 
                            WHERE ProductDetailId = @ProductDetailId AND WarehouseId = @WarehouseId";

            var p = new { ProductDetailId = productDetailId, WarehouseId = warehouseId };
            var result = await _dapperService.QueryFirstOrDefaultAsync<StockQuantity>(query, p);
            return result;
        }

        public async Task<StockDTO> GetStock(Guid productDetailId)
        {
            string query = @"
                            SELECT

                                d.Id AS ProductDetailId,
                                p.Name AS ProductName,
                                d.Price,
                                d.Color,
	                            p.ImageUrl,
	                            c.Name as CategoryName,
	                            b.Name as BrandName,
	                            -- 
	                            ISNULL(s.Id, NEWID()) as StockId,
                                wh.Id AS WarehouseId,
                                wh.Name AS WarehouseName,
                                ISNULL(s.Quantity, 0) AS Quantity

                            FROM Warehouses wh

                            LEFT JOIN ProductDetails d ON d.Id = @ProductDetailId
                            LEFT JOIN Stocks s ON s.WarehouseId = wh.Id AND s.ProductDetailId = d.Id
                            LEFT JOIN Products p ON p.Id = d.ProductId
                            LEFT JOIN Categories c ON c.Id = p.CategoryId
                            LEFT JOIN Brands b ON b.Id = p.BrandId
                            ";

            var result = await _dapperService.QueryAsync<StockDapperRow>(query, new { ProductDetailId  = productDetailId});
            if (result == null || result.Count == 0) return null;

            var stockDTO = result
                .GroupBy(g => new {g.ProductDetailId, g.ProductName, g.Color, g.Price, g.ImageUrl, g.CategoryName, g.BrandName})
                .Select(s => new StockDTO
                {
                    ProductDetailId = s.Key.ProductDetailId,
                    ProductName = s.Key.ProductName,
                    Color = s.Key.Color,
                    Price = s.Key.Price,
                    ImageUrl = s.Key.ImageUrl,
                    CategoryName = s.Key.CategoryName,
                    BrandName = s.Key.BrandName,

                    StockDetail = s.Select(d => new StockDetailDTO
                    {
                        StockId = d.StockId,
                        WarehouseId = d.WarehouseId,
                        WarehouseName = d.WarehouseName,
                        Quantity = d.Quantity,
                    }).ToList()
                })
                .FirstOrDefault();

            return stockDTO;
        }

        public async Task<StockExistDTO> GetStockExisted(Guid productDetailId)
        {
            string query = @" 
                            SELECT TOP 1
                                 wh.Id AS WarehouseId,
                                 wh.Name AS WarehouseName,
	                             d.Id as ProductDetailId,
                                 ISNULL(s.Quantity, 0) AS Quantity
                             FROM Warehouses wh

                             LEFT JOIN ProductDetails d ON d.Id = @ProductDetailId
                             LEFT JOIN Stocks s ON s.WarehouseId = wh.Id AND s.ProductDetailId = d.Id

                             WHERE Quantity > 0"
                            ;
            var p = new { ProductDetailId = productDetailId };
            var result =await _dapperService.QueryFirstOrDefaultAsync<StockExistDTO>(query, p);
            return result;
        }
    }
}
