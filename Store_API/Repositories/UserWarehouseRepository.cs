using Microsoft.EntityFrameworkCore;
using Store_API.Data;
using Store_API.Models.Users;
using Store_API.Repositories.IRepositories;
using Store_API.Infrastructures;

namespace Store_API.Repositories
{
    public class UserWarehouseRepository : Repository<UserWarehouse>, IUserWarehouseRepository
    {
        public UserWarehouseRepository(StoreContext db, IDapperService dapperService) : base(db, dapperService)
        {
        }

        public async Task<UserWarehouse> GetByUserIdAndWarehouseId(int userId, Guid warehouseId)
        {
            return await _db.UserWarehouses
                .FirstOrDefaultAsync(uw => uw.UserId == userId && uw.WarehouseId == warehouseId);
        }

        public async Task<List<Guid>> GetWarehouseIdsByUserId(int userId)
        {
            return await _db.UserWarehouses
                .Where(uw => uw.UserId == userId)
                .Select(uw => uw.WarehouseId)
                .ToListAsync();
        }
    }
} 