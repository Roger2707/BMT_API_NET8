using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Store_API.Data;
using Store_API.DTOs;
using Store_API.DTOs.Products;
using Store_API.Helpers;
using Store_API.Models;
using Store_API.Services;

namespace Store_API.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly IDapperService _dapperService;
        private readonly StoreContext _db;
        private readonly IImageRepository _imageService;
        private readonly ICSVRepository _csvService;
        public ProductRepository(IDapperService dapperService, StoreContext db, IImageRepository imageService, ICSVRepository csvService)
        {
            _dapperService = dapperService;
            _db = db;
            _imageService = imageService;
            _csvService = csvService;
        }

        #region CRUD
        public async Task<Result<Guid>> Create(ProductUpsertDTO productCreateDTO)
        {
            var product = new Product()
            {
                Name = productCreateDTO.Name,
                Description = productCreateDTO.Description,
                Created = productCreateDTO.Created,
                ProductStatus = productCreateDTO.ProductStatus,
                CategoryId = productCreateDTO.CategoryId,
                BrandId = productCreateDTO.BrandId,
                Details = productCreateDTO.ProductDetails.Select(d => new ProductDetail()
                {
                    Price = d.Price,
                    Color = d.Color,
                    ExtraName = d.ExtraName,
                    QuantityInStock = d.QuantityInStock
                }).ToList()
            };

            if (productCreateDTO.ImageUrl != null)
            {
                string folderPath = $"products/{productCreateDTO.Name.Trim().ToLower()}";

                // Upload Multiple Images
                var uploadTasks = productCreateDTO.ImageUrl
                    .Select(image =>  _imageService.AddImageAsync(image, folderPath))
                    .ToList();

                // Multi thread - run all task at the same time and wait for all to complete
                var imageResults = await Task.WhenAll(uploadTasks); 

                // Check image error != null -> save db
                var validImages = imageResults.Where(img => img.Error == null).ToList();
                product.ImageUrl = string.Join(",", validImages.Select(img => img.SecureUrl.ToString()));
                product.PublicId = string.Join(",", validImages.Select(img => img.PublicId));
            }

            await _db.Products.AddAsync(product);
            int result = await _db.SaveChangesAsync();

            if (result > 0) return Result<Guid>.Success(product.Id);
            else return Result<Guid>.Failure("Upsert Fail !");
        }

        public async Task<Result<Guid>> Update(ProductUpsertDTO productUpdateDTO)
        {
            Product existedProduct = await _db.Products.Include(p => p.Details).FirstOrDefaultAsync(p => p.Id == productUpdateDTO.Id);

            // Update Different Fields / != NULL
            if (!string.IsNullOrWhiteSpace(productUpdateDTO.Name) && productUpdateDTO.Name != existedProduct.Name)
                existedProduct.Name = productUpdateDTO.Name;
            if (!string.IsNullOrWhiteSpace(productUpdateDTO.Description) && productUpdateDTO.Description != existedProduct.Description)
                existedProduct.Description = productUpdateDTO.Description;

            if (productUpdateDTO.ProductStatus != existedProduct.ProductStatus)
                existedProduct.ProductStatus = productUpdateDTO.ProductStatus;
            if (productUpdateDTO.CategoryId != existedProduct.CategoryId)
                existedProduct.CategoryId = productUpdateDTO.CategoryId;
            if (productUpdateDTO.BrandId != existedProduct.BrandId)
                existedProduct.BrandId = productUpdateDTO.BrandId;

            // Handle Images List
            if (productUpdateDTO.ImageUrl != null)
            {
                string folderPath = $"products/{productUpdateDTO.Name.Trim().ToLower()}";

                // Upload Multiple Images
                var deleteTasks = existedProduct.PublicId.Split(new char[] { ',' })
                    .Select(publicId => _imageService.DeleteImageAsync(publicId))
                    .ToList();

                var uploadTasks = productUpdateDTO.ImageUrl
                    .Select(image => _imageService.AddImageAsync(image, folderPath))
                    .ToList();

                // Multi thread - run all task at the same time and wait for all to complete
                var imageDeletes = await Task.WhenAll(deleteTasks);
                var imageResults = await Task.WhenAll(uploadTasks);

                // Check image error != null -> save string to db
                var validImages = imageResults.Where(img => img.Error == null).ToList();
                existedProduct.ImageUrl = string.Join(",", validImages.Select(img => img.SecureUrl.ToString()));
                existedProduct.PublicId = string.Join(",", validImages.Select(img => img.PublicId));
            }

            // Handle Product Details
            _db.ProductDetails.RemoveRange(existedProduct.Details);
            await _db.AddRangeAsync(productUpdateDTO.ProductDetails.Select(d => new ProductDetail()
            {
                ProductId = existedProduct.Id,
                Price = d.Price,
                Color = d.Color,
                ExtraName = d.ExtraName,
                QuantityInStock = d.QuantityInStock
            }));

            int result = await _db.SaveChangesAsync();

            return result > 0 ? Result<Guid>.Success(existedProduct.Id) : Result<Guid>.Failure("Update Fail !");
        }

        public async Task<Result<int>> ChangeStatus(int id)
        {
            string query = @"
                            DECLARE @IsExsted INT
                            DECLARE @ProductStatus INT

                            SELECT @IsExsted = COUNT(*) FROM Products WHERE Id = @ProductId
                            IF(@IsExsted = 0 OR @IsExsted is NULL)
	                            THROW 99000, 'Product is not existed !', 1;
                            ELSE
	                            BEGIN
		                            SELECT @ProductStatus = ProductStatus FROM Products WHERE Id = @ProductId
		                            IF(@ProductStatus = 1)
			                            UPDATE Products SET ProductStatus = 2 WHERE Id = @ProductId
		                            ELSE IF(@ProductStatus = 2)
			                            UPDATE Products SET ProductStatus = 1 WHERE Id = @ProductId
		                            ELSE 
			                            THROW 99001, 'ProductStatus is not right !', 1;
	                            END";
            var p = new { ProductId = id };

            int result = await _dapperService.Execute(query, p);
            return result > 0 ? Result<int>.Success(1) : Result<int>.Failure("Can not change status !");
        }

        public async Task<Result<int>> InsertCSV(IFormFile csvFile)
        {
            if (csvFile == null || csvFile.Length == 0)
                return Result<int>.Failure("File is empty !");

            var products = await _csvService.ReadCSV<ProductCSV>(csvFile);
            string query =
                @"
                    DECLARE @IsExsted INT
                    DECLARE @Error VARCHAR(1000)

                    SELECT @IsExsted = COUNT(*) FROM Products WHERE Name = @Name
                    IF(@IsExsted > 0)
	                    BEGIN
		                    SET @Error = 'Product Name ' + CAST(@Name AS VARCHAR) + ' is existed - Import fail !!!';
		                    THROW 99000, @Error, 1;
	                    END
                    ELSE
	                    BEGIN
		                    INSERT INTO Products(Name, Price, Description, Created, QuantityInStock, ProductStatus, CategoryId, BrandId)
		                    VALUES(@Name, @Price, @Description, GETDATE(), @QuantityInStock, @ProductStatus, @CategoryId, @BrandId)
	                    END
                ";

            if (products != null && products.Count > 0)
            {
                try
                {
                    await _dapperService.BeginTransactionAsync();
                    for (int i = 0; i < products.Count; i++)
                    {
                        var p = new
                        {
                            Name = products[i].Name,
                            Price = products[i].Price,
                            Description = products[i].Description,
                            QuantityInStock = products[i].QuantityInStock,
                            ProductStatus = products[i].ProductStatus,
                            CategoryId = products[i].CategoryId,
                            BrandId = products[i].BrandId,
                        };
                        await _dapperService.Execute(query, p);
                    }
                    await _dapperService.CommitAsync();
                }
                catch (SqlException ex)
                {
                    await _dapperService.RollbackAsync();
                    return Result<int>.Failure(ex.Message);
                }
            }
            return Result<int>.Success(1);
        }

        #endregion

        #region Get Data

        public async Task<Result<ProductDTO>> GetById(Guid? id)
        {
            string query = @" 
                             SELECT 
                                product.Id
                                , product.Name
                                , detail.Id as DetailId
                                , detail.Price
                                , detail.Color
                                , detail.ExtraName
                                , IIF(promotion.PercentageDiscount is NULL
		                            , detail.Price
		                            , detail.Price - (detail.Price * (promotion.PercentageDiscount / 100))) 
	                            as DiscountPrice
                                , Description
                                , ImageUrl
                                , Created
                                , detail.QuantityInStock
                                , IIF(ProductStatus = 1, 'In Stock', 'Out Stock') as ProductStatus
                                , product.CategoryId
                                , category.Name as CategoryName
                                , product.BrandId
                                , brand.Name as BrandName
                                , brand.Country as BrandCountry

                            FROM Products as product

                            INNER JOIN Categories as category ON product.CategoryId = category.Id
                            INNER JOIN Brands as brand ON product.BrandId = brand.Id 
                            INNER JOIN ProductDetails as detail ON detail.ProductId = product.Id
                            LEFT JOIN Promotions as promotion 
                            ON product.CategoryId = promotion.CategoryId 
                                AND product.BrandId = promotion.BrandId 
                                AND promotion.[End] >= GETDATE()

                            WHERE product.Id = @Id AND product.ProductStatus = 1 ";

            var p = new { id = id };
            var result = await _dapperService.QueryAsync(query, p);
            if (result == null) return null;

            var productDTO = result
                .GroupBy(g => new {g.Id, g.Name, g.Description, g.ImageUrl, g.Created, g.ProductStatus, g.CategoryId, g.CategoryName, g.BrandId, g.BrandName, g.BrandCountry})
                .Select(g => new ProductDTO()
                {
                    Id = g.Key.Id,
                    Name = g.Key.Name,
                    Description = g.Key.Description,
                    ImageUrl = g.Key.ImageUrl,
                    Created = g.Key.Created,
                    ProductStatus = g.Key.ProductStatus,
                    CategoryId = g.Key.CategoryId,
                    CategoryName = g.Key.CategoryName,
                    BrandId = g.Key.BrandId,
                    BrandName = g.Key.BrandName,
                    BrandCountry = g.Key.BrandCountry,
                    Details = g.Select(d => new ProductDetailDTO()
                    {
                        Id = d.DetailId,
                        ProductId = g.Key.Id,
                        Color = d.Color,
                        QuantityInStock = d.QuantityInStock,
                        Price = d.Price,
                        DiscountPrice = d.DiscountPrice,
                        ExtraName = d.ExtraName
                    }).ToList()
                }).FirstOrDefault();

            return productDTO != null ? Result<ProductDTO>.Success(productDTO) : Result<ProductDTO>.Failure("Can not find product !");
        }

        #endregion

    }
}
