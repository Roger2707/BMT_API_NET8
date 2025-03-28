using Store_API.DTOs.Warehouse;

namespace Store_API.IService
{
    public interface IWarehouseService
    {
        Task<IEnumerable<WarehouseDTO>> GetAll();
        Task<WarehouseDTO> GetWarehouseDetail(Guid id);
        Task<Guid> Create(WarehouseUpsertDTO warehouseDTO);
        Task<Guid> Update(WarehouseUpsertDTO warehouseDTO);
        Task Delete(Guid id);
    }
}
