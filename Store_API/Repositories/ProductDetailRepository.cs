using Microsoft.EntityFrameworkCore;
using Store_API.Data;
using Store_API.IRepositories;
using Store_API.IService;
using Store_API.Models;

namespace Store_API.Repositories
{
    public class ProductDetailRepository : Repository<ProductDetail>, IProductDetailRepository
    {
        public ProductDetailRepository(StoreContext db, IDapperService dapperService) : base(db, dapperService)
        {
        }

        public async Task<int> ChangeProductStatus(Guid productDetailId)
        {
            var productDetail = await _db.ProductDetails.FirstOrDefaultAsync(p => p.Id == productDetailId);
            if (productDetail == null) throw new Exception("Product is not existed!");

            int currentStatus = productDetail.Status;
            productDetail.Status = currentStatus == 1 ? 2 : 1;

            return await _db.SaveChangesAsync();          
        }
    }
}
