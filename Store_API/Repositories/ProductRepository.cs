using Store_API.Data;
using Store_API.DTOs.Products;
using Store_API.Helpers;
using Store_API.Infrastructures;
using Store_API.Models;
using Store_API.Repositories.IRepositories;

namespace Store_API.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public int TotalRow { get; set; }
        public int PageSize { get; set; }

        public ProductRepository(StoreContext db, IDapperService dapperService) : base(db, dapperService)
        {
            PageSize = 10;
        }

        #region List
        public async Task<List<ProductDetailDisplayDTO>> GetProducts(ProductParams productParams)
        {
            string query = @" 
                            WITH ProductAll AS 
                            (
                                SELECT 
                                    product.Id as ProductId,
                                    detail.Id as ProductDetailId,
                                    product.Name as ProductName,
                                    detail.Color,
                                    detail.Price,
                                    ISNULL(promotion.PercentageDiscount, 0) as DiscountPercent,
                                    IIF(promotion.PercentageDiscount is NULL,
                                        detail.Price,
                                        detail.Price - (detail.Price * (promotion.PercentageDiscount / 100))) as DiscountPrice,
                                    ImageUrl,
                                    product.CategoryId,
                                    category.Name as CategoryName,
                                    product.BrandId,
                                    brand.Name as BrandName,
                                    brand.Country as BrandCountry,
                                    product.Created,
                                    ISNULL((SELECT AVG(Star) FROM Ratings WHERE ProductDetailId = detail.Id), 0) as Stars,
                                    ROW_NUMBER() OVER (PARTITION BY product.Id ORDER BY detail.Price DESC) as rn
                                FROM Products AS product
                                INNER JOIN ProductDetails AS detail ON detail.ProductId = product.Id
                                INNER JOIN Categories category ON category.Id = product.CategoryId
                                INNER JOIN Brands brand ON brand.Id = product.BrandId
                                LEFT JOIN Promotions AS promotion 
                                    ON product.CategoryId = promotion.CategoryId 
                                    AND product.BrandId = promotion.BrandId 
                                    AND promotion.EndDate >= GETDATE()
                                WHERE detail.Status = 1 -- conditions
                            ),

                            ProductFiltered AS (
                                SELECT *
                                FROM ProductAll
                                WHERE rn = 1
                            ),

                            Paged AS (
                                SELECT *,
                                       ROW_NUMBER() OVER (ORDER BY    
			                              CASE WHEN @OrderBy = '' OR @OrderBy is NULL THEN ProductName END ASC,
			                              CASE WHEN @orderBy = 'nameDesc' THEN ProductName END DESC,
			                              CASE WHEN @OrderBy = 'priceAsc' THEN DiscountPrice END ASC,
			                              CASE WHEN @OrderBy = 'priceDesc' THEN DiscountPrice END DESC
		                               ) as RowNum
                                FROM ProductFiltered
                            )

                            SELECT *,
                                   (SELECT COUNT(*) FROM ProductFiltered) AS TotalRow
                            FROM Paged
                            WHERE RowNum BETWEEN (@PageNumber - 1) * @PageSize + 1 AND @PageNumber * @PageSize

                    ";

            string where = GetConditionString(productParams);
            query = query.Replace("-- conditions", where);
            var result = await _dapperService.QueryAsync<ProductDetailDisplayDTO>(query, new { OrderBy = productParams.OrderBy, PageSize = PageSize, PageNumber = productParams.CurrentPage });

            if (result.Count == 0) return null;
            TotalRow = CF.GetInt(result[0].TotalRow);
            return result;
        }

        public async Task<List<ProductDetailDisplayDTO>> GetProductsBestSeller()
        {
            string query = @" 
                            SELECT TOP 3
                                product.Id as ProductId,
                                detail.Id as ProductDetailId,
                                product.Name as ProductName,
                                detail.Color,
                                detail.Price,
                                ISNULL(promotion.PercentageDiscount, 0) as DiscountPercent,
                                IIF(promotion.PercentageDiscount is NULL,
                                    detail.Price,
                                    detail.Price - (detail.Price * (promotion.PercentageDiscount / 100))) as DiscountPrice,
                                detail.ImageUrl,
                                product.CategoryId,
                                category.Name as CategoryName,
                                product.BrandId,
                                brand.Name as BrandName,
                                brand.Country as BrandCountry,
                                product.Created,
                                ISNULL((SELECT AVG(Star) FROM Ratings WHERE ProductDetailId = detail.Id), 0) as Stars,
                                ROW_NUMBER() OVER (PARTITION BY product.Id ORDER BY detail.Price DESC) as rn
                            FROM Products AS product
                            INNER JOIN ProductDetails AS detail ON detail.ProductId = product.Id
                            INNER JOIN Categories category ON category.Id = product.CategoryId
                            INNER JOIN Brands brand ON brand.Id = product.BrandId
                            LEFT JOIN Promotions AS promotion 
                                ON product.CategoryId = promotion.CategoryId 
                                AND product.BrandId = promotion.BrandId 
                                AND promotion.EndDate >= GETDATE()
                            WHERE detail.Status = 1 
                            ORDER BY Stars DESC
                    ";

            var result = await _dapperService.QueryAsync<ProductDetailDisplayDTO>(query);
            if (result.Count == 0) return null;
            TotalRow = CF.GetInt(result[0].TotalRow);
            return result;
        }

        // In Search Screen (Im/Ex stock)
        public async Task<IEnumerable<ProductSingleDetailDTO>> GetProductDetails(ProductSearch search)
        {
            var minPrice = CF.GetDouble(search.MinPrice);
            var maxPrice = CF.GetDouble(search.MaxPrice);
            if (minPrice > 0 && maxPrice > 0 && minPrice > maxPrice) throw new Exception("Min Price must be smaller than Max Price !");

            string query = @"
                            SELECT 
                                detail.Id as ProductDetailId,
                                product.Name as ProductName,
                                IIF(detail.ImageUrl IS NOT NULL, 
                                    (SELECT TOP 1 value FROM STRING_SPLIT(detail.ImageUrl, ',')), 
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

            if (search != null)
            {
                string where = "";
                if (!string.IsNullOrEmpty(search.ProductName)) where += " AND product.Name LIKE '%@ProductName%' ";
                if (minPrice > 0) where += " AND detail.Price >= @MinPrice ";
                if (maxPrice > 0) where += " AND detail.Price <= @MaxPrice ";
                if (search.CategoryId != Guid.Empty && search.CategoryId != null) where += " AND product.CategoryId = @CategoryId ";
                if (search.BrandId != Guid.Empty && search.BrandId != null) where += " AND product.BrandId = @BrandId ";
                query = query.Replace("-- condition", where);
            }

            var result = await _dapperService.QueryAsync<ProductSingleDetailDTO>(query, search);
            return result;
        }

        #endregion

        #region Details

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
                                , detail.ImageUrl
                                , detail.PublicId
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
                        Status = CF.GetInt(d.Status) == 1 ? "Active" : "Non - Active",
                        Price = d.Price,
                        DiscountPrice = d.DiscountPrice,
                        ExtraName = d.ExtraName
                    }).ToList()
                })
                .FirstOrDefault();

            return productDTO;
        }

        // one product can have many detail - this method returns a single detail by id
        public async Task<ProductSingleDetailDTO> GetProductSingleDetail(Guid productDetailId)
        {
            string query = @"
                SELECT 
                    detail.Id as ProductDetailId,
                    product.Name as ProductName,
                    IIF(detail.ImageUrl IS NOT NULL, 
                        (SELECT TOP 1 value FROM STRING_SPLIT(detail.ImageUrl, ',')), 
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

        #region Helper Functionalities

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

        #endregion
    }
}
