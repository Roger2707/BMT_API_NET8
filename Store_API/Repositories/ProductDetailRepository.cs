using Store_API.Data;
using Store_API.IRepositories;
using Store_API.Models;

namespace Store_API.Repositories
{
    public class ProductDetailRepository : Repository<ProductDetail>, IProductDetailRepository
    {
        public ProductDetailRepository(StoreContext db, IDapperService dapperService) : base(db, dapperService)
        {
        }
    }
}
