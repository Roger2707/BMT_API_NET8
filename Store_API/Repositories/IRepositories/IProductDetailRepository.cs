using Store_API.Models;
using Store_API.Repositories;

namespace Store_API.Repositories.IRepositories
{
    public interface IProductDetailRepository : IRepository<ProductDetail>
    {
        Task<int> ChangeProductStatus(Guid productId);
    }
}
