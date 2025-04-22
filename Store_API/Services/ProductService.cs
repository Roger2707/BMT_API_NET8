using Store_API.DTOs.Paginations;
using Store_API.DTOs.Products;
using Store_API.IService;
using Store_API.Models;
using Store_API.Repositories;

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

        public async Task<Pagination<ProductDTO>> GetPageProductDTOs(ProductParams productParams)
        {
            var products = await _unitOfWork.Product.GetProducts(productParams);
            var result = Pagination<ProductDTO>.GetPaginationData(products, _unitOfWork.Product.TotalRow, productParams.CurrentPage, 10);
            return result;
        }

        public async Task<ProductDTO> GetProductDTO(Guid productId)
        {
            var product = await _unitOfWork.Product.GetProductDTO(productId);
            return product;
        }

        public async Task<IEnumerable<ProductSingleDetailDTO>> GetProductSingleDetails(ProductSearch search)
        {
            var result = await _unitOfWork.Product.GetProductDetails(search);
            return result;
        }

        public async Task<ProductSingleDetailDTO> GetProductSingleDetail(Guid productDetailId)
        {
            var result = await _unitOfWork.Product.GetProductSingleDetail(productDetailId);
            if (result == null)
                throw new Exception($"Product detail with ID {productDetailId} not found");
            return result;
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
                        Status = d.Status
                    }).ToList()
                };

                if (!string.IsNullOrEmpty(model.ImageUrl) && !string.IsNullOrEmpty(model.PublicId))
                {
                    product.ImageUrl = model.ImageUrl;
                    product.PublicId = model.PublicId;
                }

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
            return Guid.Empty;
        }

        public async Task<Guid> UpdateProduct(ProductUpsertDTO model)
        {
            await _unitOfWork.BeginTransactionAsync(Enums.TransactionType.EntityFramework);

            try
            {
                Product existedProduct = await _unitOfWork.Product.GetByIdAsync(model.Id, p => p.Details);

                if (existedProduct == null)
                    return Guid.Empty;

                // Update Different Fields / != NULL
                if (!string.IsNullOrWhiteSpace(model.Name) && model.Name != existedProduct.Name)
                    existedProduct.Name = model.Name;
                if (!string.IsNullOrWhiteSpace(model.Description) && model.Description != existedProduct.Description)
                    existedProduct.Description = model.Description;
                if (model.CategoryId != existedProduct.CategoryId)
                    existedProduct.CategoryId = model.CategoryId;
                if (model.BrandId != existedProduct.BrandId)
                    existedProduct.BrandId = model.BrandId;

                // Handle Images List
                if (!string.IsNullOrEmpty(model.ImageUrl) && !string.IsNullOrEmpty(model.PublicId))
                {
                    existedProduct.ImageUrl = model.ImageUrl;
                    existedProduct.PublicId = model.PublicId;
                }

                // Handle Product Details
                // Remove all -> Add again
                _unitOfWork.ProductDetail.RemoveRangeAsync(existedProduct.Details);
                await _unitOfWork.ProductDetail.AddRangeAsync(model.ProductDetails.Select(d => new ProductDetail()
                {
                    Id = d.Id,
                    ProductId = d.ProductId,
                    Price = d.Price,
                    Color = d.Color,
                    ExtraName = d.ExtraName,
                    Status = d.Status
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
