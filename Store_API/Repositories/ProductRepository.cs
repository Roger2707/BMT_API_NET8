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
        public async Task<List<ProductFullDetailDTO>> GetProducts(ProductParams productParams)
        {
            string query = @" 
                        WITH FilterdProducts AS
                        (
                        SELECT
		                        product.Id as ProductId
		                        , detail.Id as ProductDetailId
		                        , product.Name
		                        , product.Description
		                        , detail.Color
		                        , detail.ExtraName
		                        , detail.Price as OriginPrice
		                        , detail.ImageUrl
		                        , detail.PublicId
		                        , category.Name as CategoryName
		                        , brand.Name as BrandName
                                , brand.Country as BrandCountry
		                        , ISNULL(
			                        (SELECT PercentageDiscount 
			                        FROM Promotions 
			                        WHERE EndDate = (SELECT Max(EndDate) FROM Promotions WHERE CategoryId = category.Id AND BrandId = brand.Id)
			                        ), 0) as PercentageDiscount
		                        , ISNULL((SELECT AVG(Star) FROM Ratings WHERE ProductDetailId = detail.Id), 5) as Stars
		                        , product.Created
		                        , detail.Status

	                        FROM ProductDetails detail

	                        INNER JOIN Products product ON product.Id = detail.ProductId
	                        INNER JOIN Categories category ON category.Id = product.CategoryId
	                        INNER JOIN Brands brand ON brand.Id = product.BrandId

	                        WHERE detail.Status = 1 -- conditions 
                        )

                        SELECT 
	                        *
	                        , IIF(p.PercentageDiscount > 0, p.OriginPrice - p.OriginPrice * p.PercentageDiscount / 100, p.OriginPrice) as DiscountPrice
	                        , (SELECT COUNT(1) FROM FilterdProducts) as TotalRow
                        FROM FilterdProducts as p
                        ORDER BY p.Created
                        OFFSET @Offset ROWS
                        FETCH NEXT @FetchNext ROWS ONLY
                    ";

            string where = GetConditionString(productParams);
            query = query.Replace("-- conditions", where);
            int offset = PageSize * (productParams.CurrentPage - 1);
            int fetchNext = PageSize;
            var result = await _dapperService.QueryAsync<ProductFullDetailDTO>(query, new { Offset = offset, FetchNext = fetchNext });

            if (result.Count == 0) return null;
            TotalRow = CF.GetInt(result[0].TotalRow);
            return result;
        }

        public async Task<List<ProductFullDetailDTO>> GetProductsBestSeller()
        {
            string query = @" 
                            WITH FilterdProducts AS
                            (
                            SELECT
		                            product.Id as ProductId
		                            , detail.Id as ProductDetailId
		                            , product.Name
		                            , product.Description
		                            , detail.Color
		                            , detail.ExtraName
		                            , detail.Price as OriginPrice
		                            , detail.ImageUrl
		                            , detail.PublicId
		                            , category.Name as CategoryName
		                            , brand.Name as BrandName
                                    , brand.Country as BrandCountry
		                            , ISNULL(
			                            (SELECT PercentageDiscount 
			                            FROM Promotions 
			                            WHERE EndDate = (SELECT Max(EndDate) FROM Promotions WHERE CategoryId = category.Id AND BrandId = brand.Id)
			                            ), 0) as PercentageDiscount
		                            , ISNULL((SELECT AVG(Star) FROM Ratings WHERE ProductDetailId = detail.Id), 5) as Stars
		                            , product.Created
		                            , detail.Status

	                            FROM ProductDetails detail

	                            INNER JOIN Products product ON product.Id = detail.ProductId
	                            INNER JOIN Categories category ON category.Id = product.CategoryId
	                            INNER JOIN Brands brand ON brand.Id = product.BrandId

	                            WHERE detail.Status = 1
                            )

                            SELECT TOP 3
	                            *
	                            , IIF(p.PercentageDiscount > 0, p.OriginPrice - p.OriginPrice * p.PercentageDiscount / 100, p.OriginPrice) as DiscountPrice
                                , (SELECT COUNT(1) FROM FilterdProducts) as TotalRow
                            FROM FilterdProducts as p
                            ORDER BY p.Stars DESC
                    ";

            var result = await _dapperService.QueryAsync<ProductFullDetailDTO>(query);
            if (result.Count == 0) return null;
            TotalRow = CF.GetInt(result[0].TotalRow);
            return result;
        }

        #endregion

        #region Product -> n Detail

        public async Task<ProductDTO> GetProductDTO(Guid productId)
        {
            string query = @" 
                        WITH FilterdProducts AS
                        (
                        SELECT
		                        product.Id as ProductId
		                        , detail.Id as ProductDetailId
		                        , product.Name
		                        , product.Description
		                        , detail.Color
		                        , detail.ExtraName
		                        , detail.Price as OriginPrice
		                        , detail.ImageUrl
		                        , detail.PublicId
		                        , category.Name as CategoryName
		                        , brand.Name as BrandName
                                , brand.Country as BrandCountry
		                        , ISNULL(
			                        (SELECT PercentageDiscount 
			                        FROM Promotions 
			                        WHERE EndDate = (SELECT Max(EndDate) FROM Promotions WHERE CategoryId = category.Id AND BrandId = brand.Id)
			                        ), 0) as PercentageDiscount
		                        , ISNULL((SELECT AVG(Star) FROM Ratings WHERE ProductDetailId = detail.Id), 5) as Stars
		                        , product.Created
		                        , detail.Status

	                        FROM ProductDetails detail

	                        INNER JOIN Products product ON product.Id = detail.ProductId
	                        INNER JOIN Categories category ON category.Id = product.CategoryId
	                        INNER JOIN Brands brand ON brand.Id = product.BrandId

	                        WHERE detail.Status = 1 AND product.Id = @ProductId
                        )

                        SELECT 
	                        *
	                        , IIF(p.PercentageDiscount > 0, p.OriginPrice - p.OriginPrice * p.PercentageDiscount / 100, p.OriginPrice) as DiscountPrice
                            , (SELECT COUNT(1) FROM FilterdProducts) as TotalRow
                        FROM FilterdProducts as p
                        ORDER BY p.Created
                            ";

            var p = new { ProductId = productId };
            var result = await _dapperService.QueryAsync<ProductFullDetailDTO>(query, p);
            if (result == null) return null;

            var productDTO = result
                .GroupBy(g => new 
                { g.ProductId, g.Name, g.Description, g.Created, g.CategoryName, g.BrandName, g.BrandCountry, g.TotalRow })
                .Select(g => new ProductDTO()
                {
                    Id = g.Key.ProductId,
                    Name = g.Key.Name,
                    Description = g.Key.Description,
                    Created = g.Key.Created,
                    CategoryName = g.Key.CategoryName,
                    BrandName = g.Key.BrandName,
                    BrandCountry = g.Key.BrandCountry,
                    TotalRow = g.Key.TotalRow,

                    Details = g.Select(d => new ProductDetailDTO()
                    {
                        Id = d.ProductDetailId,
                        ProductId = g.Key.ProductId,
                        Color = d.Color,
                        Status = "Active",
                        ImageUrl = d.ImageUrl,
                        PublicId = d.PublicId,
                        OriginPrice = d.OriginPrice,
                        PercentageDiscount = d.PercentageDiscount,
                        DiscountPrice = d.DiscountPrice,
                        ExtraName = d.ExtraName,
                        Stars = d.Stars

                    }).ToList()
                })
                .FirstOrDefault();

            return productDTO;
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

            if(productParams.MinPrice > 0)
                where += " AND detail.Price >= " + productParams.MinPrice + " ";

            if (productParams.MaxPrice > 0)
                where += " AND detail.Price <= " + productParams.MaxPrice + " ";

            return where;
        }

        #endregion
    }
}
