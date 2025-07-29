using Store_API.Data;
using Store_API.Infrastructures;
using Store_API.Models.Users;
using Store_API.Repositories.IRepositories;

namespace Store_API.Repositories
{
    public class RoleRepository : Repository<Role>, IRoleRepository
    {
        public RoleRepository(StoreContext db, IDapperService dapperService) : base(db, dapperService)
        {
        }
    }
}
