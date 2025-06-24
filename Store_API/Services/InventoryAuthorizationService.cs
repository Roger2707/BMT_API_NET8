using Microsoft.AspNetCore.Identity;
using Store_API.Models.Users;
using Store_API.IService;
using Store_API.IRepositories;

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
        public async Task<bool> IsSuperAdmin(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return false;

            return await _userManager.IsInRoleAsync(user, "SuperAdmin");
        }
        public async Task<bool> IsAdmin(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return false;

            return await _userManager.IsInRoleAsync(user, "Admin");
        }
        public async Task<bool> IsWarehouseAdmin(int userId, Guid warehouseId)
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