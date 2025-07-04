using Store_API.DTOs.Paginations;
using Store_API.DTOs.Products;
using Store_API.Models;
using Store_API.Services.IService;
using Store_API.Infrastructures;

namespace Store_API.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region Retrieve Data

        public async Task<Pagination<ProductFullDetailDTO>> GetPageProducts(ProductParams productParams)
        {
            var products = await _unitOfWork.Product.GetProducts(productParams);
            var result = Pagination<ProductFullDetailDTO>.GetPaginationData(products, _unitOfWork.Product.TotalRow, productParams.CurrentPage, _unitOfWork.Product.PageSize);
            return result;
        }

        public async Task<ProductDTO> GetProductDTO(Guid productId)
        {
            var product = await _unitOfWork.Product.GetProductDTO(productId);
            return product;
        }

        public async Task<List<ProductFullDetailDTO>> GetProductsBestSeller()
        {
            var bestSellers = await _unitOfWork.Product.GetProductsBestSeller();
            return bestSellers;
        }

        #endregion

        #region CRUD Operations

        public async Task<Guid> CreateProduct(ProductUpsertDTO model)
        {
            await _unitOfWork.BeginTransactionAsync(Enums.TransactionType.EntityFramework);
            try
            {
                var product = new Product()
                {
                    Id = model.Id,
                    Name = model.Name,
                    Description = model.Description,
                    Created = model.Created,
                    CategoryId = model.CategoryId,
                    BrandId = model.BrandId,

                    Details = model.ProductDetails.Select(d => new ProductDetail()
                    {
                        Price = d.Price,
                        Color = d.Color,
                        ExtraName = d.ExtraName,
                        Status = d.Status,
                        ImageUrl = !string.IsNullOrEmpty(d.ImageUrl) ? d.ImageUrl : "",
                        PublicId = !string.IsNullOrEmpty(d.PublicId) ? d.PublicId : "",
                    }).ToList()
                };

                await _unitOfWork.Product.AddAsync(product);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();

                return model.Id;
            }
            catch(Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<Guid> UpdateProduct(ProductUpsertDTO model)
        {
            await _unitOfWork.BeginTransactionAsync(Enums.TransactionType.EntityFramework);

            try
            {
                Product existedProduct = await _unitOfWork.Product.FindFirstAsync(p => p.Id == model.Id, p => p.Details);
                if (existedProduct == null) return Guid.Empty;

                existedProduct.Name = model.Name ?? existedProduct.Name;
                existedProduct.Description = model.Description ?? existedProduct.Description;
                existedProduct.CategoryId = model.CategoryId != existedProduct.CategoryId ? model.CategoryId : existedProduct.CategoryId;
                existedProduct.BrandId = model.BrandId != existedProduct.BrandId ? model.BrandId : existedProduct.BrandId;

                _unitOfWork.ProductDetail.RemoveRangeAsync(existedProduct.Details);
                await _unitOfWork.ProductDetail.AddRangeAsync(model.ProductDetails.Select(d => new ProductDetail()
                {
                    Id = d.Id,
                    ProductId = d.ProductId,
                    Price = d.Price,
                    Color = d.Color,
                    ExtraName = d.ExtraName,
                    Status = d.Status,
                    ImageUrl = !string.IsNullOrEmpty(d.ImageUrl) ? d.ImageUrl : "",
                    PublicId = !string.IsNullOrEmpty(d.PublicId) ? d.PublicId : "",
                }));

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();
                return model.Id;
            }
            catch(Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<int> UpdateProductStatus(Guid productId)
        {
            int result = await _unitOfWork.ProductDetail.ChangeProductStatus(productId);
            return result;
        }

        #endregion
    }
}
