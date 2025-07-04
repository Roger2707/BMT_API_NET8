using Store_API.DTOs.Products;
using Store_API.Models;

namespace Store_API.Repositories.IRepositories
{
    public interface IProductRepository : IRepository<Product>
    {
        public int TotalRow { get; set; }
        public int PageSize { get; set; }
        Task<List<ProductFullDetailDTO>> GetProducts(ProductParams productParams);
        Task<List<ProductFullDetailDTO>> GetProductsBestSeller();
        Task<ProductDTO> GetProductDTO(Guid productId);
    }
}
