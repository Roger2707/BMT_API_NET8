using Store_API.Data;
using Store_API.DTOs.Brands;
using Store_API.Helpers;
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
