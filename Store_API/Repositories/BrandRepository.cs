using Store_API.Data;
using Store_API.IRepositories;
using Store_API.IService;
using Store_API.Models;

namespace Store_API.Repositories
{
    public class BrandRepository : Repository<Brand>, IBrandRepository
    {
        public BrandRepository(StoreContext db, IDapperService dapperService) : base(db, dapperService)
        {

        }      
    }
}
