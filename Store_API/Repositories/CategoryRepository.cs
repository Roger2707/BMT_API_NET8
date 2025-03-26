using Microsoft.EntityFrameworkCore;
using Store_API.Data;
using Store_API.Helpers;
using Store_API.Models;

namespace Store_API.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(StoreContext db, IDapperService dapperService) : base(db, dapperService)
        {

        }
    }
}
