using Store_API.Data;
using Store_API.Infrastructures;
using Store_API.Models;
using Store_API.Repositories.IRepositories;

namespace Store_API.Repositories
{
    public class BrandRepository : Repository<Brand>, IBrandRepository
    {
        public BrandRepository(StoreContext db, IDapperService dapperService) : base(db, dapperService)
        {

        }      
    }
}
