using Store_API.Data;
using Store_API.IRepositories;
using Store_API.Models.Users;

namespace Store_API.Repositories
{
    public class UserAddressRepository : Repository<UserAddress>, IUserAddressRepository
    {
        public UserAddressRepository(StoreContext db, IDapperService dapperService) : base(db, dapperService)
        {

        }
    }
}
