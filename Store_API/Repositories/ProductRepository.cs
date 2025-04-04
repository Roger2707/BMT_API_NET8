using Store_API.Data;
using Store_API.DTOs.Products;
using Store_API.Helpers;
using Store_API.Models;

namespace Store_API.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        protected readonly int count_in_page = 10;
        public ProductRepository(StoreContext db, IDapperService dapperService) : base(db, dapperService)
        {

        }

        #region Retrieve Data
        public async Task<List<ProductDTO>> GetProducts(ProductParams productParams)
        {
            string query = @" 
                            WITH ProductPagination AS 
                            (
	                            SELECT 
                                    product.Id
                                    , product.Name
                                    , Description
                                    , ImageUrl
                                    , PublicId
                                    , Created
                                    , product.CategoryId
                                    , category.Name as CategoryName
                                    , product.BrandId
                                    , brand.Name as BrandName
                                    , brand.Country as BrandCountry

                                FROM Products as product

                                INNER JOIN Categories as category ON product.CategoryId = category.Id
                                INNER JOIN Brands as brand ON product.BrandId = brand.Id 

	                            WHERE 1 = 1 
	                            -- where                         
                                -- orderBy
	                            OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY
                            )
                            SELECT
	                            page.*
                                , detail.Id as DetailId
                                , detail.Price
                                , detail.Color
                                , detail.ExtraName
	                            , detail.Status
                                , IIF(promotion.PercentageDiscount is NULL
                                    , detail.Price
                                    , detail.Price - (detail.Price * (promotion.PercentageDiscount / 100))) 
                                as DiscountPrice

                            FROM ProductPagination as page
                            INNER JOIN ProductDetails detail ON detail.ProductId = page.Id
                            LEFT JOIN Promotions as promotion 
	                            ON page.CategoryId = promotion.CategoryId 
		                            AND page.BrandId = promotion.BrandId 
		                            AND promotion.EndDate >= GETDATE()

                            -- sortDetailPrice
                            WHERE detail.Status = 1
";

            string where = GetConditionString(productParams);
            string orderBy = GetOrderByString(productParams.OrderBy);

            if(!string.IsNullOrEmpty(productParams.OrderBy) && productParams.OrderBy.Contains("price"))
            {
                query = query.Replace("-- orderBy", "ORDER BY product.Name");
                query = query.Replace("-- sortDetailPrice", orderBy);
            }
            else query = query.Replace("-- orderBy", orderBy);

            query = query.Replace("-- where", where);

            int skip = productParams.CurrentPage == 1 ? 0 : count_in_page * (productParams.CurrentPage - 1);
            int take = count_in_page;

            var result = await _dapperService.QueryAsync<ProductDapperRow>(query, new { Skip = skip, Take = take });

            if (result.Count == 0) return null;

            var products = result
                .GroupBy(g => new { g.Id, g.Name, g.Description, g.ImageUrl, g.Created, g.CategoryId, g.CategoryName, g.BrandId, g.BrandName, g.BrandCountry })
                .Select(s => new ProductDTO
                {
                    Id = s.Key.Id,
                    Name = s.Key.Name,
                    Description = s.Key.Description,
                    ImageUrl = s.Key.ImageUrl,
                    Created = s.Key.Created,
                    CategoryId = s.Key.CategoryId,
                    CategoryName = s.Key.CategoryName,
                    BrandId = s.Key.BrandId,
                    BrandName = s.Key.BrandName,
                    BrandCountry = s.Key.BrandCountry,
                    Details = s.Select(d => new ProductDetailDTO
                    {
                        Id = d.DetailId,
                        ProductId = s.Key.Id,
                        Color = d.Color,
                        Price = CF.GetDouble(d.Price),
                        DiscountPrice = CF.GetDouble(d.DiscountPrice),
                        Status = CF.GetInt(d.Status) == 1 ? "In stock" : "Out Stock",
                        ExtraName = d.ExtraName
                    }).ToList()
                })
                .ToList();

            return products;
        }

        public async Task<ProductDTO> GetProductDTO(Guid id)
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
                                , PublicId
                                , Created
                                , detail.Status
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
                                AND promotion.EndDate >= GETDATE()

                            WHERE product.Id = @Id AND detail.Status = 1 ";

            var p = new { id = id };
            var result = await _dapperService.QueryAsync<ProductDapperRow>(query, p);
            if (result == null) return null;

            var productDTO = result
                .GroupBy(g => new { g.Id, g.Name, g.Description, g.ImageUrl, g.Created, g.CategoryId, g.CategoryName, g.BrandId, g.BrandName, g.BrandCountry })
                .Select(g => new ProductDTO()
                {
                    Id = g.Key.Id,
                    Name = g.Key.Name,
                    Description = g.Key.Description,
                    ImageUrl = g.Key.ImageUrl,
                    Created = g.Key.Created,
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
                        Status = CF.GetInt(d.Status) == 1 ? "In stock" : "Out Stock",
                        Price = d.Price,
                        DiscountPrice = d.DiscountPrice,
                        ExtraName = d.ExtraName
                    }).ToList()
                })
                .FirstOrDefault();

            return productDTO;
        }

        public async Task<IEnumerable<ProductSingleDetailDTO>> GetProductDetails(ProductSearch search)
        {
            var minPrice = CF.GetDouble(search.MinPrice);
            var maxPrice = CF.GetDouble(search.MaxPrice);
            if (minPrice > 0 && maxPrice > 0 && minPrice > maxPrice) throw new Exception("Min Price must be smaller than Max Price !");

            string query = @"
                SELECT 
                    detail.Id as ProductDetailId,
                    product.Name as ProductName,
                    IIF(product.ImageUrl IS NOT NULL, 
                        (SELECT TOP 1 value FROM STRING_SPLIT(product.ImageUrl, ',')), 
                        '') AS ProductFirstImage,
                    detail.Price as OriginPrice,
                    ISNULL(promotion.PercentageDiscount, 0) as DiscountPercent,
                    IIF(promotion.PercentageDiscount is not NULL, 
                        detail.Price - (detail.Price * (promotion.PercentageDiscount / 100)), 
                        detail.Price) as DiscountPrice,
                    brand.Name as BrandName,
                    category.Name as CategoryName
                FROM ProductDetails detail
                INNER JOIN Products product ON detail.ProductId = product.Id
                INNER JOIN Categories category ON category.Id = product.CategoryId
                INNER JOIN Brands brand ON brand.Id = product.BrandId
                LEFT JOIN Promotions promotion 
                    ON promotion.CategoryId = category.Id 
                    AND promotion.BrandId = brand.Id 
                    AND promotion.EndDate >= GETDATE()
                WHERE 1 = 1 -- condition";

            if(search != null)
            {
                string where = "";
                if(!string.IsNullOrEmpty(search.ProductName)) where += " AND product.Name LIKE '%@ProductName%' ";
                if(minPrice > 0) where += " AND detail.Price >= @MinPrice ";
                if(maxPrice > 0) where += " AND detail.Price <= @MaxPrice ";
                if(search.CategoryId != Guid.Empty && search.CategoryId != null) where += " AND product.CategoryId = @CategoryId ";
                if(search.BrandId != Guid.Empty && search.BrandId != null) where += " AND product.BrandId = @BrandId ";
                query = query.Replace("-- condition", where);
            }

            var result = await _dapperService.QueryAsync<ProductSingleDetailDTO>(query, search);
            return result;
        }

        public async Task<ProductSingleDetailDTO> GetProductSingleDetail(Guid productDetailId)
        {
            string query = @"
                SELECT 
                    detail.Id as ProductDetailId,
                    product.Name as ProductName,
                    IIF(product.ImageUrl IS NOT NULL, 
                        (SELECT TOP 1 value FROM STRING_SPLIT(product.ImageUrl, ',')), 
                        '') AS ProductFirstImage,
                    detail.Price as OriginPrice,
                    ISNULL(promotion.PercentageDiscount, 0) as DiscountPercent,
                    IIF(promotion.PercentageDiscount is not NULL, 
                        detail.Price - (detail.Price * (promotion.PercentageDiscount / 100)), 
                        detail.Price) as DiscountPrice,
                    brand.Name as BrandName,
                    category.Name as CategoryName
                FROM ProductDetails detail
                INNER JOIN Products product ON detail.ProductId = product.Id
                INNER JOIN Categories category ON category.Id = product.CategoryId
                INNER JOIN Brands brand ON brand.Id = product.BrandId
                LEFT JOIN Promotions promotion 
                    ON promotion.CategoryId = category.Id 
                    AND promotion.BrandId = brand.Id 
                    AND promotion.EndDate >= GETDATE()
                WHERE detail.Id = @ProductDetailId";

            var result = await _dapperService.QueryFirstOrDefaultAsync<ProductSingleDetailDTO>(query, new { ProductDetailId = productDetailId });
            return result;
        }

        #endregion

        #region Other Functionalities

        public async Task<int> GetNumbersRecord(ProductParams productParams)
        {
            string query = @"   
                            WITH ProductIdsGroup AS 

                            (
                            SELECT product.Id

                            FROM Products as product

                            INNER JOIN ProductDetails detail ON product.Id = detail.ProductId
                            INNER JOIN Categories as category ON product.CategoryId = category.Id
                            INNER JOIN Brands as brand ON product.BrandId = brand.Id 
                            LEFT JOIN Promotions as promotion 

                            ON product.CategoryId = promotion.CategoryId 
                                AND product.BrandId = promotion.BrandId 
                                AND promotion.EndDate <= GETDATE()

                            WHERE detail.Status = 1
                            -- where

                            GROUP BY product.Id
                            )

                            SELECT COUNT(1) as TotalRecords FROM ProductIdsGroup
                            ";

            string where = GetConditionString(productParams);
            query = query.Replace("-- where", where);

            int result = await _dapperService.QueryFirstOrDefaultAsync<int>(query, null);
            return result;
        }

        private static string GetConditionString(ProductParams productParams)
        {
            string where = "";

            if (!string.IsNullOrEmpty(productParams.SearchBy))
            {
                where += " AND product.Name LIKE '%" + productParams.SearchBy + "%' ";
            }

            if (!string.IsNullOrEmpty(productParams.FilterByCategory))
            {
                where += " AND product.CategoryId IN(" + productParams.FilterByCategory + ") ";
            }

            if (!string.IsNullOrEmpty(productParams.FilterByBrand))
            {
                where += " AND product.BrandId IN(" + productParams.FilterByBrand + ") ";
            }

            return where;
        }

        private static string GetOrderByString(string paramsOrderBy)
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
                    orderBy = " ORDER BY detail.Price ASC ";
                    break;
                case "priceDESC":
                    orderBy = " ORDER BY detail.Price DESC ";
                    break;
            }

            return orderBy;
        }

        #endregion
    }
}
