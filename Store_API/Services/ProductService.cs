using Dapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Store_API.Data;
using Store_API.DTOs;
using Store_API.DTOs.Products;
using Store_API.Helpers;
using Store_API.Models;
using Store_API.Repositories;

namespace Store_API.Services
{
    public class ProductService : IProductRepository
    {
        private readonly int count_in_page = 6;

        private readonly StoreContext _db;
        private readonly IDapperService _dapperService;
        private readonly IImageRepository _imageService;
        public ProductService(StoreContext db, IDapperService dapperService, IImageRepository imageService)
        {
            _db = db;
            _dapperService = dapperService;
            _imageService = imageService;
        }

        #region CRUD
        public async Task Create(ProductUpsertDTO productCreateDTO)
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
                var imageResult = await _imageService.AddImageAsync(productCreateDTO.ImageUrl);

                if (imageResult.Error != null) throw new Exception(imageResult.Error.Message);

                product.ImageUrl = imageResult.SecureUrl.ToString();
                product.PublicId = imageResult.PublicId;
            }

            await _db.Products.AddAsync(product);
        }

        public async Task<int> InsertCSV(ProductCSV productCSV)
        {
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
            var p = new 
            {
                Name = productCSV.Name,
                Price = productCSV.Price,
                Description = productCSV.Description,
                QuantityInStock = productCSV.QuantityInStock,
                ProductStatus = productCSV.ProductStatus,
                CategoryId = productCSV.CategoryId,
                BrandId = productCSV.BrandId,
            };

            int result = await _dapperService.Execute(query, p);
            return result;
        }

        public async Task<Product> Update(int id, ProductUpsertDTO productUpdateDTO)
        {
            Product existedProduct = await _db.Products.FindAsync(id);

            if(productUpdateDTO.Name != "" && productUpdateDTO.Name != existedProduct.Name)
                existedProduct.Name = productUpdateDTO.Name;
            if(productUpdateDTO.Price != existedProduct.Price && productUpdateDTO.Price > 0) 
                existedProduct.Price = productUpdateDTO.Price;
            if(productUpdateDTO.Description != "") 
                existedProduct.Description = productUpdateDTO.Description;
            if(productUpdateDTO.QuantityInStock != existedProduct.QuantityInStock) 
                existedProduct.QuantityInStock = productUpdateDTO.QuantityInStock;
            if(productUpdateDTO.ProductStatus != existedProduct.ProductStatus) 
                existedProduct.ProductStatus = productUpdateDTO.ProductStatus;
            if(productUpdateDTO.CategoryId != existedProduct.CategoryId) 
                existedProduct.CategoryId = productUpdateDTO.CategoryId;
            if(productUpdateDTO.BrandId != existedProduct.BrandId)
                existedProduct.BrandId = productUpdateDTO.BrandId;

            if (productUpdateDTO.ImageUrl != null)
            {
                var imageUploadResult = await _imageService.AddImageAsync(productUpdateDTO.ImageUrl);

                if (imageUploadResult.Error != null)
                    throw new Exception(imageUploadResult.Error.Message);

                if (!string.IsNullOrEmpty(existedProduct.PublicId))
                    await _imageService.DeleteImageAsync(existedProduct.PublicId);

                existedProduct.ImageUrl = imageUploadResult.SecureUrl.ToString();
                existedProduct.PublicId = imageUploadResult.PublicId;
            }

            return existedProduct;
        }

        public async Task<int> ChangeStatus(int id)
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
            return result;
        }

        #endregion

        #region Get Data

        public async Task<List<ProductDTO>> GetAll(ProductParams productParams)
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
                                AND promotion.[End] <= GETDATE()

                            WHERE product.ProductStatus = 1 

                            --where
                            --orderBy";

            string where = GetConditionString(productParams);
            string orderBy = GetOrderByString(productParams.OrderBy);

            query = query.Replace("--orderBy", orderBy);  
            query = query.Replace("--where", where);
            List<dynamic> result = await _dapperService.QueryAsync(query, null);

            if (result.Count == 0) return null;

            List<ProductDTO> products = new List<ProductDTO>();

            for (int i = 0; i < result.Count; i++)
            {
                var product = new ProductDTO
                {
                    Id = result[i].Id,  
                    Name = result[i].Name,
                    Price = CF.GetDouble(result[i].Price),
                    DiscountPrice = CF.GetDouble(result[i].DiscountPrice),
                    Description = result[i].Description,
                    ImageUrl = result[i].ImageUrl,
                    Created = result[i].Created,
                    QuantityInStock = result[i].QuantityInStock,
                    ProductStatus = result[i].ProductStatus,
                    CategoryId = result[i].CategoryId,
                    CategoryName = result[i].CategoryName,
                    BrandId = result[i].BrandId,
                    BrandName = result[i].BrandName,
                    BrandCountry = result[i].BrandCountry,
                };

                products.Add(product);
            }

            return products;
        }

        public async Task<ProductDTO> GetById(int id)
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
                                AND promotion.[End] <= GETDATE()

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

            return product;
        }

        public async Task<int> GetTotalRecord()
        {
            string query = @" SELECT COUNT(*) as TotalRow FROM Products ";
            dynamic result = await _dapperService.QueryFirstOrDefaultAsync(query, null);
            return CF.GetInt(result?.TotalRow);
        }

        public async Task<List<ProductDTO>> GetSourceProducts(ProductParams productParams)
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
                                AND promotion.[End] <= GETDATE()

                            WHERE product.ProductStatus = 1 

                            --where
                            
                            --orderBy

                            OFFSET @Skip ROWS
                            FETCH NEXT @Take ROWS ONLY;

";

            string where = GetConditionString(productParams);
            string orderBy = GetOrderByString(productParams.OrderBy);

            query = query.Replace("--orderBy", orderBy);
            query = query.Replace("--where", where);

            int skip = productParams.CurrentPage == 1 ? 0 : count_in_page * (productParams.CurrentPage - 1);
            int take = count_in_page;

            List<dynamic> result = await _dapperService.QueryAsync(query, new { Skip = skip, Take = take });

            if (result.Count == 0) return null;

            List<ProductDTO> products = new List<ProductDTO>();

            for (int i = 0; i < result.Count; i++)
            {
                var product = new ProductDTO
                {
                    Id = result[i].Id,
                    Name = result[i].Name,
                    Price = CF.GetDouble(result[i].Price),
                    DiscountPrice = CF.GetDouble(result[i].DiscountPrice),
                    Description = result[i].Description,
                    ImageUrl = result[i].ImageUrl,
                    Created = result[i].Created,
                    QuantityInStock = result[i].QuantityInStock,
                    ProductStatus = result[i].ProductStatus,
                    CategoryId = result[i].CategoryId,
                    CategoryName = result[i].CategoryName,
                    BrandId = result[i].BrandId,
                    BrandName = result[i].BrandName,
                    BrandCountry = result[i].BrandCountry,
                };

                products.Add(product);
            }

            return products;
        }

        public async Task<Pagination<ProductDTO>> GetPagination(List<ProductDTO> products, int currentPage)
        {
            int totalRow = await GetTotalRecord();
            Pagination<ProductDTO> productPagination = Pagination<ProductDTO>.GetPaginationData(products, totalRow, currentPage, count_in_page);
            return productPagination;
        }

        #endregion


        #region Helpers

        private string GetConditionString(ProductParams productParams)
        {
            string where = "";

            if (!string.IsNullOrEmpty(productParams.SearchBy))
            {
                where += " AND product.Name LIKE '%" + productParams.SearchBy + "%' ";
            }

            if (!string.IsNullOrEmpty(productParams.FilterByCategory))
            {
                where += " AND product.CategoryId IN(" + productParams.FilterByCategory +") ";
            }

            if (!string.IsNullOrEmpty(productParams.FilterByBrand))
            {
                where += " AND product.BrandId IN(" + productParams.FilterByBrand + ") ";
            }

            return where;
        }

        private string GetOrderByString(string paramsOrderBy)
        {
            string orderBy = "";

            switch (paramsOrderBy)
            {
                case "":
                case null:
                    orderBy = " ORDER BY product.Name ";
                    break;
                case "NameDesc":
                    orderBy = " ORDER BY product.Name DESC ";
                    break;
                case "priceASC":
                    orderBy = " ORDER BY product.Price ";
                    break;
                case "priceDESC":
                    orderBy = " ORDER BY product.Price DESC ";
                    break;
            }

            return orderBy;
        }

        #endregion

    }
}
