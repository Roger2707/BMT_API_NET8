using Store_API.DTOs.Products;
using Store_API.DTOs;

namespace Store_API.IService
{
    public interface IProductService
    {
        public Task<int> GetTotalRecord(ProductParams productParams);
        public Task<List<ProductDTO>> GetSourceProducts(ProductParams productParams);
        public Task<Result<Pagination<ProductDTO>>> GetPagination(List<ProductDTO> products, ProductParams productParams);
        public Task<dynamic> GetTechnologies(int productId);
    }
}
