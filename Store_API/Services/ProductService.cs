using Store_API.Data;
using Store_API.DTOs;
using Store_API.DTOs.Products;
using Store_API.Helpers;
using Store_API.IService;
using Store_API.Repositories;

namespace Store_API.Services
{
    public class ProductService : IProductService
    {
        private readonly int count_in_page = 10;

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

        #endregion

        #region Search / Sort / Filter

        public async Task<int> GetTotalRecord(ProductParams productParams)
        {
            string query = @"   
                                SELECT COUNT(1) as TotalRow 

                                FROM Products as product

                                INNER JOIN Categories as category ON product.CategoryId = category.Id
                                INNER JOIN Brands as brand ON product.BrandId = brand.Id 
                                LEFT JOIN Promotions as promotion 

                                ON product.CategoryId = promotion.CategoryId 
                                    AND product.BrandId = promotion.BrandId 
                                    AND promotion.[End] <= GETDATE()

                                WHERE product.ProductStatus = 1 

                                --where
                            ";

            string where = GetConditionString(productParams);
            query = query.Replace("--where", where);
            dynamic result = await _dapperService.QueryFirstOrDefaultAsync(query, null);
            return CF.GetInt(result?.TotalRow);
        }

        public async Task<List<ProductDTO>> GetSourceProducts(ProductParams productParams)
        {
            string query = @" 
                            WITH ProductPagination AS 
                            (
	                            SELECT 
		                            product.Id
		                            , product.Name
		                            , Description
		                            , ImageUrl
		                            , Created
		                            , IIF(ProductStatus = 1, 'In Stock', 'Out Stock') as ProductStatus
		                            , product.CategoryId
		                            , category.Name as CategoryName
		                            , product.BrandId
		                            , brand.Name as BrandName
		                            , brand.Country as BrandCountry

	                            FROM Products as product

	                            INNER JOIN Categories as category ON product.CategoryId = category.Id
	                            INNER JOIN Brands as brand ON product.BrandId = brand.Id 

	                            WHERE product.ProductStatus = 1 
	                            --where                         
                                --orderBy
	                            OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY
                            )
                            SELECT
	                            page.*
	                            , detail.Id as DetailId
	                            , detail.Price
	                            , detail.Color
	                            , detail.ExtraName
	                            , IIF(promotion.PercentageDiscount is NULL
		                            , detail.Price
		                            , detail.Price - (detail.Price * (promotion.PercentageDiscount / 100))) 
	                            as DiscountPrice
	                            , detail.QuantityInStock
                            FROM ProductPagination as page
                            INNER JOIN ProductDetails detail ON detail.ProductId = page.Id
                            LEFT JOIN Promotions as promotion 
	                            ON page.CategoryId = promotion.CategoryId 
		                            AND page.BrandId = promotion.BrandId 
		                            AND promotion.[End] >= GETDATE()

";

            string where = GetConditionString(productParams);
            string orderBy = GetOrderByString(productParams.OrderBy);

            query = query.Replace("--orderBy", orderBy);
            query = query.Replace("--where", where);

            int skip = productParams.CurrentPage == 1 ? 0 : count_in_page * (productParams.CurrentPage - 1);
            int take = count_in_page;

            List<dynamic> result = await _dapperService.QueryAsync(query, new { Skip = skip, Take = take });

            if (result.Count == 0) return null;

            var products = result
                .GroupBy(g => new { g.Id, g.Name, g.Description, g.ImageUrl, g.Created, g.ProductStatus, g.CategoryId, g.CategoryName, g.BrandId, g.BrandName, g.BrandCountry })
                .Select(s => new ProductDTO
                {
                    Id = s.Key.Id,
                    Name = s.Key.Name,
                    Description = s.Key.Description,
                    ImageUrl = s.Key.ImageUrl,
                    Created = s.Key.Created,
                    ProductStatus = s.Key.ProductStatus,
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
                        QuantityInStock = CF.GetInt(d.QuantityInStock),
                        ExtraName = d.ExtraName
                    }).ToList()
                }).ToList();

            return products;
        }

        public async Task<Result<Pagination<ProductDTO>>> GetPagination(List<ProductDTO> products, ProductParams productParams)
        {
            int totalRow = await GetTotalRecord(productParams);
            var productPagination = Pagination<ProductDTO>.GetPaginationData(products, totalRow, productParams.CurrentPage, count_in_page);
            return productPagination != null ? Result<Pagination<ProductDTO>>.Success(productPagination) : Result<Pagination<ProductDTO>>.Failure("");
        }

        public async Task<dynamic> GetTechnologies(int productId)
        {
            string query = @"SELECT 
	                                t.Name
	                                , t.Description
	                                , t.ImageUrl
                                FROM Technologies t
                                INNER JOIN ProductTechnology pt ON t.Id = pt.TechnologiesId
                                WHERE pt.ProductsId = @ProductId
                                ";

            List<dynamic> result = await _dapperService.QueryAsync(query, new { ProductId = productId });
            var teches = new List<dynamic>();

            if (result.Count > 0)
            {
                for (int i = 0; i < result.Count; i++)
                    teches.Add(new { Name = result[i].Name, Description = result[i].Description, ImageUrl = result[i].ImageUrl });
            }
            return teches;
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
                where += " AND product.CategoryId IN(" + productParams.FilterByCategory + ") ";
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
