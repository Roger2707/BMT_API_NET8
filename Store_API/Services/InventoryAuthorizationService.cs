using Store_API.Services.IService;
using Store_API.Infrastructures;

namespace Store_API.Services
{
    public class InventoryAuthorizationService : IInventoryAuthorization
    {
        private readonly IUnitOfWork _unitOfWork;

        public InventoryAuthorizationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> IsSuperAdmin(int userId)
        {
            var user = await _unitOfWork.User.FindFirstAsync(u => u.Id == userId);
            if (user == null) return false;

            return await _unitOfWork.User.CheckRoleAsync(user.Id, 1);
        }
        public async Task<bool> IsAdmin(int userId)
        {
            var user = await _unitOfWork.User.FindFirstAsync(u => u.Id == userId);
            if (user == null) return false;

            return await _unitOfWork.User.CheckRoleAsync(user.Id, 2);
        }
        public async Task<bool> IsWarehouseAdmin(int userId, Guid warehouseId)
        {
            var user = await _unitOfWork.User.FindFirstAsync(u => u.Id == userId);
            if (user == null) return false;

            // Check Admin role
            if (!await _unitOfWork.User.CheckRoleAsync(user.Id, 2))
                return false;

            // Check UserWarehouse table
            var userWarehouse = await _unitOfWork.UserWarehouse.FindFirstAsync(ua => ua.UserId == userId && ua.WarehouseId == warehouseId);
            return userWarehouse != null;
        }
    }
} 