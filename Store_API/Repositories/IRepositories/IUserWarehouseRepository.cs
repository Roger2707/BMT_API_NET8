using Store_API.Models.Users;
using Store_API.Repositories;

namespace Store_API.Repositories.IRepositories
{
    public interface IUserWarehouseRepository : IRepository<UserWarehouse>
    {
        Task<UserWarehouse> GetByUserIdAndWarehouseId(int userId, Guid warehouseId);
        Task<List<Guid>> GetWarehouseIdsByUserId(int userId);
    }
}