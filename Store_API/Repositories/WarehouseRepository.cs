using Store_API.Data;
using Store_API.DTOs.Warehouse;
using Store_API.IRepositories;
using Store_API.Models.Inventory;

namespace Store_API.Repositories
{
    public class WarehouseRepository : Repository<Warehouse>, IWarehouseRepository
    {
        public WarehouseRepository(StoreContext db, IDapperService dapperService) : base(db, dapperService)
        {

        }

        public async Task<List<WarehouseProductQuantity>> GetProductQuantityInWarehouse(Guid productDetalId)
        {
            string query = @"
                            SELECT
	                            wh.Id as WarehouseId
	                            , wh.Name as WarehouseName
	                            , ISNULL(s.Quantity, 0) as Quantity

                            FROM Warehouses wh
                            LEFT JOIN Stocks s ON wh.Id = s.WarehouseId AND s.ProductDetailId = @ProductDetailId
                            LEFT JOIN ProductDetails detail ON s.ProductDetailId = detail.Id 
                            ";
            var result = await _dapperService.QueryAsync<WarehouseProductQuantity>(query, new { ProductDetailId = productDetalId });
            return result;
        }
    }
}
