using Store_API.Data;
using Store_API.DTOs.Promotions;
using Store_API.Models;

namespace Store_API.Repositories
{
    public class PromotionRepository : Repository<Promotion>, IPromotionRepository
    {
        public PromotionRepository(StoreContext db, IDapperService dapperService) : base(db, dapperService)
        {

        }

        public async Task<IEnumerable<PromotionDTO>> GetAllPromotionsAsync()
        {
            string query = @"
                            SELECT
	                            promotion.Id
	                            , promotion.Start as StartDate
	                            , promotion.[End] as EndDate
	                            , promotion.BrandId
	                            , brand.Name as BrandName
	                            , promotion.CategoryId 
	                            , category.Name as CategoryName
	                            , promotion.PercentageDiscount

                            FROM Promotions promotion

                            INNER JOIN Brands brand ON promotion.BrandId = brand.Id
                            INNER JOIN Categories category ON promotion.CategoryId = category.Id
                            ";
            var promotions = await QueryAsync<PromotionDTO>(query);
            return promotions;
        }

        public async Task<PromotionDTO> GetPromotionAsync(Guid promotionId)
        {
            string query = @"
                            SELECT
	                            promotion.Id
	                            , promotion.Start as StartDate
	                            , promotion.[End] as EndDate
	                            , promotion.BrandId
	                            , brand.Name as BrandName
	                            , promotion.CategoryId 
	                            , category.Name as CategoryName
	                            , promotion.PercentageDiscount

                            FROM Promotions promotion

                            INNER JOIN Brands brand ON promotion.BrandId = brand.Id
                            INNER JOIN Categories category ON promotion.CategoryId = category.Id

                            WHERE promotion.Id = @PromotionId
                            ";
            var promotion = await QueryFirstOrDefaultAsyncAsync<PromotionDTO>(query, new {PromotionId = promotionId});
            return promotion;
        }

        public async Task<DateTime?> GetMaxEndDateOfOnePromotionAsync(Guid categoryId, Guid brandId)
        {
            string query = @" SELECT MAX([End]) as MaxEndDate from Promotions WHERE CategoryId = @CategoryId and BrandId = @BrandId ";
            var p = new { CategoryId = categoryId, BrandId = brandId };
            var maxEndDate = await _dapperService.QueryFirstOrDefaultAsync<DateTime?>(query, p);
            return maxEndDate;
        }
    }
}
