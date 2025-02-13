using Microsoft.Data.SqlClient;
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
        public async Task<Result<int>> Create(ProductUpsertDTO productCreateDTO)
        {
            var product = new Product()
            {
                Name = productCreateDTO.Name,
                Price = productCreateDTO.Price,
                Description = productCreateDTO.Description,
                Created = productCreateDTO.Created,
                QuantityInStock = productCreateDTO.QuantityInStock,
                ProductStatus = productCreateDTO.ProductStatus,
                CategoryId = productCreateDTO.CategoryId,
                BrandId = productCreateDTO.BrandId,
            };

            if (productCreateDTO.ImageUrl != null)
            {
                string folderPath = $"products/{productCreateDTO.Name.Trim().ToLower()}";
                var imageResults = await _imageService.AddMultipleImageAsync(productCreateDTO.ImageUrl, folderPath);

                int index = 0;
                foreach (var imageResult in imageResults)
                {
                    if (imageResult.Error != null) throw new Exception(imageResult.Error.Message);

                    if (index == imageResults.Count - 1)
                    {
                        product.ImageUrl += imageResult.SecureUrl.ToString();
                        product.PublicId += imageResult.PublicId;
                    }
                    else
                    {
                        product.ImageUrl += imageResult.SecureUrl.ToString() + ",";
                        product.PublicId += imageResult.PublicId + ",";
                    }
                    index++;
                }
            }

            await _db.Products.AddAsync(product);
            int result = await _db.SaveChangesAsync();

            if (result > 0) return Result<int>.Success(product.Id);
            else return Result<int>.Failure("Upsert Fail !");
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

            if(products != null && products.Count > 0)
            {
                try
                {
                    _dapperService.BeginTrans();
                    for(int i = 0; i < products.Count; i ++)
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
                    _dapperService.Commit();
                }
                catch(SqlException ex)
                {
                    _dapperService.Rollback();
                    return Result<int>.Failure(ex.Message);
                }
            }
            return Result<int>.Success(1);         
        }

        public async Task<Result<int>> Update(int id, ProductUpsertDTO productUpdateDTO)
        {
            Product existedProduct = await _db.Products.FindAsync(id);

            // Update Different Fields / != NULL
            if (productUpdateDTO.Name != "" && productUpdateDTO.Name != existedProduct.Name)
                existedProduct.Name = productUpdateDTO.Name;
            if (productUpdateDTO.Price != existedProduct.Price && productUpdateDTO.Price > 0)
                existedProduct.Price = productUpdateDTO.Price;
            if (productUpdateDTO.Description != "")
                existedProduct.Description = productUpdateDTO.Description;
            if (productUpdateDTO.QuantityInStock != existedProduct.QuantityInStock)
                existedProduct.QuantityInStock = productUpdateDTO.QuantityInStock;
            if (productUpdateDTO.ProductStatus != existedProduct.ProductStatus)
                existedProduct.ProductStatus = productUpdateDTO.ProductStatus;
            if (productUpdateDTO.CategoryId != existedProduct.CategoryId)
                existedProduct.CategoryId = productUpdateDTO.CategoryId;
            if (productUpdateDTO.BrandId != existedProduct.BrandId)
                existedProduct.BrandId = productUpdateDTO.BrandId;

            if (productUpdateDTO.ImageUrl != null)
            {
                string folderPath = $"products/{productUpdateDTO.Name.Trim().ToLower()}";

                // 1. Add Image 
                var imageResults = await _imageService.AddMultipleImageAsync(productUpdateDTO.ImageUrl, folderPath);

                // 2. Remove Product's Old Images on Cloudinary 
                if (!string.IsNullOrEmpty(existedProduct.PublicId))
                    await _imageService.DeleteMultipleImageAsync(existedProduct.PublicId);

                // 3. Fill Props into Product
                int index = 0;
                existedProduct.ImageUrl = "";
                existedProduct.PublicId = "";

                foreach (var imageResult in imageResults)
                {
                    if (imageResult.Error != null) throw new Exception(imageResult.Error.Message);

                    if (index == imageResults.Count - 1)
                    {
                        existedProduct.ImageUrl += imageResult.SecureUrl.ToString();
                        existedProduct.PublicId += imageResult.PublicId;
                    }
                    else
                    {
                        existedProduct.ImageUrl += imageResult.SecureUrl.ToString() + ",";
                        existedProduct.PublicId += imageResult.PublicId + ",";
                    }
                    index++;
                }
            }
            int result = await _db.SaveChangesAsync();

            return result > 0 ? Result<int>.Success(1) : Result<int>.Failure("Update Fail !");
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

        #endregion

        #region Get Data

        public async Task<Result<ProductDTO>> GetById(int id)
        {
            string query = @" 
                             SELECT 
                                product.Id
                                , product.Name
                                , product.Price
	                            , product.Price - (product.Price * (promotion.PercentageDiscount / 100)) as DiscountPrice
                                , Description
                                , ImageUrl
                                , Created
                                , QuantityInStock
                                , IIF(ProductStatus = 1, 'In Stock', 'Out Stock') as ProductStatus
                                , product.CategoryId
                                , category.Name as CategoryName
                                , product.BrandId
                                , brand.Name as BrandName
                                , brand.Country as BrandCountry

                            FROM Products as product

                            INNER JOIN Categories as category ON product.CategoryId = category.Id
                            INNER JOIN Brands as brand ON product.BrandId = brand.Id 
                            LEFT JOIN Promotions as promotion 
                            ON product.CategoryId = promotion.CategoryId 
                                AND product.BrandId = promotion.BrandId 
                                AND promotion.[End] >= GETDATE()

                            WHERE product.Id = @Id AND product.ProductStatus = 1 ";

            var p = new { id = id };
            var result = await _dapperService.QueryFirstOrDefaultAsync(query, p);
            if (result == null) return null;

            ProductDTO product = new()
            {
                Id = result.Id,
                Name = result.Name,
                Price = CF.GetDouble(result.Price),
                DiscountPrice = CF.GetDouble(result.DiscountPrice),
                Description = result.Description,
                ImageUrl = result.ImageUrl,
                Created = result.Created,
                QuantityInStock = result.QuantityInStock,
                ProductStatus = result.ProductStatus,
                CategoryId = result.CategoryId,
                CategoryName = result.CategoryName,
                BrandId = result.BrandId,
                BrandName = result.BrandName,
                BrandCountry = result.BrandCountry,
            };

            return product != null ? Result<ProductDTO>.Success(product) : Result<ProductDTO>.Failure("Can not find product !");
        }

        #endregion


    }
}
