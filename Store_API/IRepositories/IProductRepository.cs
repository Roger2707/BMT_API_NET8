using Store_API.DTOs.Products;
using Store_API.Models;

namespace Store_API.Repositories
{
    public interface IProductRepository : IRepository<Product>
    {
        public int TotalRow { get; set; }
        public int PageSize { get; set; }
        Task<List<ProductDTO>> GetProducts(ProductParams productParams);
        Task<ProductDTO> GetProductDTO(Guid id);
        Task<IEnumerable<ProductSingleDetailDTO>> GetProductDetails(ProductSearch search);
        Task<ProductSingleDetailDTO> GetProductSingleDetail(Guid productDetailId);
    }
}
