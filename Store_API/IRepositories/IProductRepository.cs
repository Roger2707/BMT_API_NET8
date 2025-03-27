using Store_API.DTOs.Products;
using Store_API.Models;

namespace Store_API.Repositories
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<List<ProductDTO>> GetProducts(ProductParams productParams);
        Task<ProductDTO> GetProductDTODetail(Guid id);
        Task<int> ChangeProductStatus(Guid productId);
        Task<int> GetNumbersRecord(ProductParams productParams);
    }
}
