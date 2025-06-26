using Store_API.Data;
using Store_API.Models.Users;
using Store_API.Repositories.IRepositories;
using Store_API.Services.IService;

namespace Store_API.Repositories
{
    public class UserAddressRepository : Repository<UserAddress>, IUserAddressRepository
    {
        public UserAddressRepository(StoreContext db, IDapperService dapperService) : base(db, dapperService)
        {

        }
    }
}
