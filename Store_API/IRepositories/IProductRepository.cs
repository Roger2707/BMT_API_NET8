using Store_API.DTOs.Products;
using Store_API.Models;

namespace Store_API.Repositories
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<List<ProductDTO>> GetProducts(ProductParams productParams);
        Task<ProductDTO> GetProductDTODetail(Guid id);
        Task<ProductWithDetailDTO> GetProductWithDetail(Guid productDetailId);
        Task<int> GetNumbersRecord(ProductParams productParams);
    }
}
