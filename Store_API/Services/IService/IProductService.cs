using Store_API.DTOs.Products;
using Store_API.DTOs.Paginations;

namespace Store_API.Services.IService
{
    public interface IProductService
    {
        Task<Pagination<ProductDetailDisplayDTO>> GetPageProductDTOs(ProductParams productParams);
        Task<List<ProductDetailDisplayDTO>> GetProductsBestSeller();
        Task<ProductDTO> GetProductDTO(Guid productId);
        Task<IEnumerable<ProductSingleDetailDTO>> GetProductSingleDetails(ProductSearch search);
        Task<ProductSingleDetailDTO> GetProductSingleDetail(Guid productDetailId);
        Task<Guid> CreateProduct(ProductUpsertDTO model);
        Task<Guid> UpdateProduct(ProductUpsertDTO model);
        Task<int> UpdateProductStatus(Guid productId);
    }
}
