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
        private readonly IInventoryAuthorization _authorizationService;

        public WarehouseService(IUnitOfWork unitOfwork, IInventoryAuthorization authorizationService)
        {
            _unitOfwork = unitOfwork;
            _authorizationService = authorizationService;
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
                IsSuperAdminOnly = w.IsSuperAdminOnly
            }).ToList();
        }

        public async Task<WarehouseDTO> GetWarehouseDetail(Guid id)
        {
            var warehouseExisted = await _unitOfwork.Warehouse.GetByIdAsync(id);
            if (warehouseExisted == null) throw new Exception("Warehouse not found");

            return new WarehouseDTO
            {
                Id = id,
                Name = warehouseExisted.Name,
                Location = warehouseExisted.Location,
                Created = warehouseExisted.Created,
                IsSuperAdminOnly = warehouseExisted.IsSuperAdminOnly
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
                IsSuperAdminOnly = warehouseDTO.IsSuperAdminOnly
            };
            await _unitOfwork.Warehouse.AddAsync(warehouse);
            await _unitOfwork.SaveChangesAsync();
            return warehouseDTO.Id;
        }

        public async Task<Guid> Update(WarehouseUpsertDTO warehouseDTO, int userId)
        {
            var warehouseExisted = await _unitOfwork.Warehouse.GetByIdAsync(warehouseDTO.Id);
            if (warehouseExisted == null) throw new Exception("Warehouse is not existed !");

            // Only SuperAdmin can modify SuperAdmin-only warehouses
            if (warehouseExisted.IsSuperAdminOnly)
            {
                if (!await _authorizationService.IsSuperAdmin(userId))
                {
                    throw new UnauthorizedAccessException("Only SuperAdmin can modify SuperAdmin-only warehouses");
                }
            }

            warehouseExisted.Name = warehouseDTO.Name;
            warehouseExisted.Location = warehouseDTO.Location;
            warehouseExisted.IsSuperAdminOnly = warehouseDTO.IsSuperAdminOnly;

            _unitOfwork.Warehouse.UpdateAsync(warehouseExisted);
            await _unitOfwork.SaveChangesAsync();
            return warehouseExisted.Id;
        }

        public async Task Delete(Guid id, int userId)
        {
            var warehouseExisted = await _unitOfwork.Warehouse.GetByIdAsync(id);
            if (warehouseExisted == null) throw new Exception("Warehouse is not existed !");

            // Only SuperAdmin can delete SuperAdmin-only warehouses
            if (warehouseExisted.IsSuperAdminOnly)
            {
                if (!await _authorizationService.IsSuperAdmin(userId))
                {
                    throw new UnauthorizedAccessException("Only SuperAdmin can delete SuperAdmin-only warehouses");
                }
            }

            _unitOfwork.Warehouse.DeleteAsync(warehouseExisted);
            await _unitOfwork.SaveChangesAsync();
        }

        #endregion
    }
}
