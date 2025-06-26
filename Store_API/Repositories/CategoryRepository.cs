using Store_API.Data;
using Store_API.Infrastructures;
using Store_API.Models;
using Store_API.Repositories.IRepositories;

namespace Store_API.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(StoreContext db, IDapperService dapperService) : base(db, dapperService)
        {

        }
    }
}
