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
    }
} 