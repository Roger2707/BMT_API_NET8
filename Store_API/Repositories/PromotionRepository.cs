using Store_API.Data;
using Store_API.DTOs.Promotions;
using Store_API.Models;
using Store_API.Repositories.IRepositories;
using Store_API.Services.IService;

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
	                            , promotion.StartDate
	                            , promotion.EndDate
	                            , promotion.BrandId
	                            , brand.Name as BrandName
	                            , promotion.CategoryId 
	                            , category.Name as CategoryName
	                            , promotion.PercentageDiscount

                            FROM Promotions promotion

                            INNER JOIN Brands brand ON promotion.BrandId = brand.Id
                            INNER JOIN Categories category ON promotion.CategoryId = category.Id

                            ORDER BY promotion.StartDate ASC
                            ";
            var promotions = await QueryAsync<PromotionDTO>(query);
            return promotions;
        }

        public async Task<DateTime?> GetMaxEndDateOfOnePromotionAsync(Guid categoryId, Guid brandId)
        {
            string query = @" SELECT MAX(EndDate) as MaxEndDate from Promotions WHERE CategoryId = @CategoryId and BrandId = @BrandId ";
            var p = new { CategoryId = categoryId, BrandId = brandId };
            var maxEndDate = await _dapperService.QueryFirstOrDefaultAsync<DateTime?>(query, p);
            return maxEndDate;
        }
    }
}
