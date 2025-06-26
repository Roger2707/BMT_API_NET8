using Store_API.DTOs.Warehouse;

namespace Store_API.Services.IService
{
    public interface IWarehouseService
    {
        Task<IEnumerable<WarehouseDTO>> GetAll();
        Task<WarehouseDTO> GetWarehouseDetail(Guid id);
        Task<List<WarehouseProductQuantity>> GetProductQuantityInWarehouse(Guid productDetalId);
        Task<Guid> Create(WarehouseUpsertDTO warehouseDTO);
        Task<Guid> Update(WarehouseUpsertDTO warehouseDTO, int userId);
        Task Delete(Guid id, int userId);
    }
}
