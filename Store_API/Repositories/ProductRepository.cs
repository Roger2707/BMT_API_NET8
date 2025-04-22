using Store_API.Data;
using Store_API.DTOs.Products;
using Store_API.Helpers;
using Store_API.Models;

namespace Store_API.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        protected readonly int count_in_page = 10;
        public int TotalRow { get; set; }

        public ProductRepository(StoreContext db, IDapperService dapperService) : base(db, dapperService)
        {

        }

        #region Retrieve Data
        public async Task<List<ProductDTO>> GetProducts(ProductParams productParams)
        {
            string query = @" 
                        WITH ProductBase AS (
	                        SELECT 
		                        product.Id
		                        , product.Name 
		                        , product.Created
		                        , category.Name as CategoryName
		                        , brand.Name as BrandName
		                        , ROW_NUMBER() OVER (
			                        ORDER BY 
				                        CASE WHEN @SortBy = 'price' THEN MIN(detail.Price) END,
				                        CASE WHEN @SortBy = 'priceDesc' THEN MAX(detail.Price) END DESC,
				                        CASE WHEN @SortBy = 'nameDesc' THEN product.Name END DESC,
				                        CASE WHEN (@SortBy = '' OR @SortBy IS NULL) THEN product.Name END
		                        ) as RowNumber

	                        FROM Products product
	                        LEFT JOIN Categories category ON category.Id = product.CategoryId
	                        LEFT JOIN Brands brand ON brand.Id = product.BrandId
	                        LEFT JOIN ProductDetails detail ON detail.ProductId = product.Id
	                        WHERE detail.Status = 1
	                        GROUP BY product.Id, product.Name, product.Created, category.Name, brand.Name
                        )

                        , PagedProductIds AS (
	                        SELECT Id
	                        FROM ProductBase
	                        WHERE RowNumber > @PageSize * (@PageNumber - 1) AND RowNumber <= @PageSize * @PageNumber
                        )

                        SELECT 
	                        product.Id
	                        , product.Name 
	                        , product.Description
	                        , product.ImageUrl
	                        , product.PublicId
	                        , product.CategoryId
	                        , product.BrandId
	                        , product.Created
	                        , category.Name as CategoryName
	                        , brand.Name as BrandName
	                        , brand.Country as BrandCountry
	                        , detail.Id as DetailId
	                        , detail.Price
	                        , detail.Color
	                        , detail.ExtraName
	                        , detail.Status
	                        , IIF(promotion.PercentageDiscount IS NULL, detail.Price, detail.Price - (detail.Price * promotion.PercentageDiscount / 100.0)) as DiscountPrice

                        FROM Products product

                        LEFT JOIN Categories category ON category.Id = product.CategoryId
                        LEFT JOIN Brands brand ON brand.Id = product.BrandId
                        LEFT JOIN ProductDetails detail ON detail.ProductId = product.Id
                        LEFT JOIN Promotions promotion 
	                        ON promotion.CategoryId = product.CategoryId
	                        AND promotion.BrandId = product.BrandId
	                        AND promotion.EndDate >= GETDATE()

                        WHERE detail.Status = 1 AND product.Id IN (SELECT Id FROM PagedProductIds)

                            ";

            string where = GetConditionString(productParams);
            query = query.Replace("-- conditions", where);
            var result = await _dapperService.QueryAsync<ProductDapperRow>(query, new { PageSize = 10, PageNumber = productParams.CurrentPage, SortBy = productParams.OrderBy });

            if (result.Count == 0) return null;

            var products = result
                .GroupBy(g => new { g.Id, g.Name, g.Description, g.ImageUrl, g.Created, g.CategoryId, g.CategoryName, g.BrandId, g.BrandName, g.BrandCountry, g.TotalRow })
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
            TotalRow = CF.GetInt(result[0].TotalRow);
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
                                detail.Color as Color,
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
                    detail.Color as Color,
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
                string categoryFilter = productParams.FilterByCategory.Trim();

                if (categoryFilter.Contains(','))
                {
                    categoryFilter = categoryFilter.Replace(",", "','");
                }
                categoryFilter = $"'{categoryFilter}'";

                where += " AND product.CategoryId IN(" + categoryFilter + ") ";
            }

            if (!string.IsNullOrEmpty(productParams.FilterByBrand))
            {
                string brandyFilter = productParams.FilterByBrand.Trim();

                if (brandyFilter.Contains(','))
                {
                    brandyFilter = brandyFilter.Replace(",", "','");
                }
                brandyFilter = $"'{brandyFilter}'";

                where += " AND product.BrandId IN(" + brandyFilter + ") ";
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
