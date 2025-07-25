using Store_API.DTOs.Products;
using Store_API.DTOs.Paginations;

namespace Store_API.Services.IService
{
    public interface IProductService
    {
        Task<Pagination<ProductFullDetailDTO>> GetPageProducts(ProductParams productParams);
        Task<ProductDTO> GetProductDTO(Guid productId);
        Task<List<ProductFullDetailDTO>> GetProductsBestSeller();

        #region CRUD
        Task<bool> CreateProduct(ProductUpsertDTO model);
        Task<bool> UpdateProduct(ProductUpsertDTO model, Guid updatedProductId);
        Task<int> UpdateProductStatus(Guid productId);
        #endregion
    }
}
