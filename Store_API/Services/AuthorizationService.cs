using Microsoft.AspNetCore.Identity;
using Store_API.Models.Users;
using Store_API.IService;
using Store_API.Repositories;

namespace Store_API.Services
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;

        public AuthorizationService(IUnitOfWork unitOfWork, UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<bool> HasWarehouseAccess(int userId, Guid warehouseId)
        {
            // 1. Kiểm tra SuperAdmin
            if (await IsSuperAdmin(userId))
                return true;

            // 2. Kiểm tra Admin và warehouse
            return await IsWarehouseAdmin(userId, warehouseId);
        }

        public async Task<bool> HasSpecialAccess(int userId, Guid warehouseId, string permission)
        {
            // 1. Kiểm tra SuperAdmin
            if (await IsSuperAdmin(userId))
                return true;

            // 2. Kiểm tra quyền đặc biệt
            switch (permission)
            {
                case Permission.TRANSFER_STOCK:
                    return await CanTransferStock(userId, warehouseId);
                case Permission.DELETE_WAREHOUSE:
                    return await CanDeleteWarehouse(userId, warehouseId);
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
            // 1. Nếu là SuperAdmin, trả về tất cả warehouse
            if (await IsSuperAdmin(userId))
            {
                var userWarehouses = await _unitOfWork.UserWarehouse.GetAllAsync();
                return userWarehouses.Select(uw => uw.WarehouseId).ToList();
            }

            // 2. Lấy danh sách warehouse của user
            return await _unitOfWork.UserWarehouse.GetWarehouseIdsByUserId(userId);
        }

        private async Task<bool> IsSuperAdmin(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return false;

            return await _userManager.IsInRoleAsync(user, "SuperAdmin");
        }

        private async Task<bool> IsWarehouseAdmin(int userId, Guid warehouseId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return false;

            // Kiểm tra role Admin
            if (!await _userManager.IsInRoleAsync(user, "Admin"))
                return false;

            // Kiểm tra trong bảng UserWarehouse
            var userWarehouse = await _unitOfWork.UserWarehouse.GetByUserIdAndWarehouseId(userId, warehouseId);
            return userWarehouse != null;
        }

        private async Task<bool> CanTransferStock(int userId, Guid warehouseId)
        {
            // Logic kiểm tra quyền chuyển kho
            // Có thể thêm các điều kiện đặc biệt
            return await IsWarehouseAdmin(userId, warehouseId);
        }

        private async Task<bool> CanDeleteWarehouse(int userId, Guid warehouseId)
        {
            // Logic kiểm tra quyền xóa warehouse
            // Có thể thêm các điều kiện đặc biệt
            return await IsSuperAdmin(userId); // Chỉ SuperAdmin mới có quyền xóa warehouse
        }
    }
} 