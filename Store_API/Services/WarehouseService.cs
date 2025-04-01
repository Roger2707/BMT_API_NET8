using Dapper;
using Store_API.DTOs.Warehouse;
using Store_API.IService;
using Store_API.Models.Inventory;
using Store_API.Repositories;

namespace Store_API.Services
{
    public class WarehouseService : IWarehouseService
    {
        private readonly IUnitOfWork _unitOfwork;

        public WarehouseService(IUnitOfWork unitOfwork)
        {
            _unitOfwork = unitOfwork;
        }

        #region Retrieve

        public async Task<IEnumerable<WarehouseDTO>> GetAll()
        {
            var warehouses = await _unitOfwork.Warehouse.GetAllAsync();
            return warehouses.Select(w => new WarehouseDTO
            {
                Id = w.Id,
                Name = w.Name,
                Location = w.Location,
                Created = w.Created,
            }).ToList();
        }

        public async Task<WarehouseDTO> GetWarehouseDetail(Guid id)
        {
            var warehouseExisted = await _unitOfwork.Warehouse.GetByIdAsync(id);
            return new WarehouseDTO
            {
                Id = id,
                Name = warehouseExisted.Name,
                Location = warehouseExisted.Location,
                Created = DateTime.UtcNow,
            };
        }

        public async Task<List<WarehouseProductQuantity>> GetProductQuantityInWarehouse(Guid productDetalId)
        {
            var result = await _unitOfwork.Warehouse.GetProductQuantityInWarehouse(productDetalId);
            return result.AsList();
        }

        #endregion

        #region CRUD Operations
        public async Task<Guid> Create(WarehouseUpsertDTO warehouseDTO)
        {
            var warehouse = new Warehouse()
            {
                Id = warehouseDTO.Id,
                Name = warehouseDTO.Name,
                Location = warehouseDTO.Location,
                Created = DateTime.UtcNow,
            };
            await _unitOfwork.Warehouse.AddAsync(warehouse);
            await _unitOfwork.SaveChangesAsync();
            return warehouseDTO.Id;
        }

        public async Task<Guid> Update(WarehouseUpsertDTO warehouseDTO)
        {
            var warehouseExisted = await _unitOfwork.Warehouse.GetByIdAsync(warehouseDTO.Id);
            if (warehouseExisted == null) throw new Exception("Warehouse is not existed !");

            warehouseExisted.Name = warehouseDTO.Name;
            warehouseExisted.Location = warehouseDTO.Location;

            _unitOfwork.Warehouse.UpdateAsync(warehouseExisted);
            await _unitOfwork.SaveChangesAsync();
            return warehouseExisted.Id;
        }

        public async Task Delete(Guid id)
        {
            var warehouseExisted = await _unitOfwork.Warehouse.GetByIdAsync(id);
            if (warehouseExisted == null) throw new Exception("Warehouse is not existed !");
            _unitOfwork.Warehouse.DeleteAsync(warehouseExisted);
            await _unitOfwork.SaveChangesAsync();
        }

        #endregion
    }
}
