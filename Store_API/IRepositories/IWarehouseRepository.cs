using Store_API.DTOs.Warehouse;
using Store_API.Models.Inventory;
using Store_API.Repositories;

namespace Store_API.IRepositories
{
    public interface IWarehouseRepository : IRepository<Warehouse>
    {
        Task<List<WarehouseProductQuantity>> GetProductQuantityInWarehouse(Guid productDetalId);
    }
}
