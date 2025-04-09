using Microsoft.AspNetCore.Identity;
using Store_API.Models.Users;
using Store_API.IService;
using Store_API.Repositories;

namespace Store_API.Services
{
    public class InventoryAuthorizationService : IInventoryAuthorization
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;

        public InventoryAuthorizationService(IUnitOfWork unitOfWork, UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<bool> HasWarehouseAccess(int userId, Guid warehouseId)
        {
            // 1. Check if SuperAdmin
            if (await IsSuperAdmin(userId))
                return true;

            // 2. Check Admin and warehouse
            return await IsWarehouseAdmin(userId, warehouseId);
        }

        public async Task<bool> HasSpecialAccess(int userId, Guid warehouseId, string permission)
        {
            // 1. Check if SuperAdmin
            if (await IsSuperAdmin(userId))
                return true;

            // 2. Check special permissions
            switch (permission)
            {
                case Permission.MANAGE_WAREHOUSE:
                    return await IsWarehouseAdmin(userId, warehouseId);
                case Permission.VIEW_WAREHOUSE:
                    return await IsWarehouseAdmin(userId, warehouseId);

                case Permission.MANAGE_STOCK:
                case Permission.IMPORT_STOCK:
                case Permission.EXPORT_STOCK:
                case Permission.VIEW_STOCK:
                    return await IsWarehouseAdmin(userId, warehouseId);
                default:
                    return false;
            }
        }

        public async Task<List<Guid>> GetUserWarehouses(int userId)
        {
            // 1. If SuperAdmin, return all warehouses
            if (await IsSuperAdmin(userId))
            {
                var userWarehouses = await _unitOfWork.UserWarehouse.GetAllAsync();
                return userWarehouses.Select(uw => uw.WarehouseId).ToList();
            }

            // 2. Get user's warehouses
            return await _unitOfWork.UserWarehouse.GetWarehouseIdsByUserId(userId);
        }

        public async Task<bool> IsSuperAdmin(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return false;

            return await _userManager.IsInRoleAsync(user, "SuperAdmin");
        }

        private async Task<bool> IsWarehouseAdmin(int userId, Guid warehouseId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return false;

            // Check Admin role
            if (!await _userManager.IsInRoleAsync(user, "Admin"))
                return false;

            // Check UserWarehouse table
            var userWarehouse = await _unitOfWork.UserWarehouse.GetByUserIdAndWarehouseId(userId, warehouseId);
            return userWarehouse != null;
        }
    }
} 