using Store_API.DTOs.Products;
using Store_API.DTOs.Paginations;

namespace Store_API.IService
{
    public interface IProductService
    {
        Task<Pagination<ProductDTO>> GetPageProductDTOs(ProductParams productParams);
        Task<ProductDTO> GetProductDTO(Guid productId);
        Task<IEnumerable<ProductSingleDetailDTO>> GetProductSingleDetails(ProductSearch search);
        Task<ProductSingleDetailDTO> GetProductSingleDetail(Guid productDetailId);
        Task<Guid> CreateProduct(ProductUpsertDTO model);
        Task<Guid> UpdateProduct(ProductUpsertDTO model);
        Task<int> UpdateProductStatus(Guid productId);
    }
}
