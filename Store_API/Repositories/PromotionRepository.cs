using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Store_API.Data;
using Store_API.DTOs.Promotions;
using Store_API.Helpers;
using Store_API.Models;

namespace Store_API.Repositories
{
    public class PromotionRepository : IPromotionRepository
    {
        private readonly StoreContext _db;
        private readonly IDapperService _dapperService;
        public PromotionRepository(StoreContext db, IDapperService dapperService)
        {
            _db = db;
            _dapperService = dapperService;
        }
        public async Task Create(PromotionUpsertDTO promotion)
        {
            try
            {
                if (promotion.Start.Length != 8 || promotion.End.Length != 8)
                    throw new Exception("Date is invalid");

                DateTime start = new DateTime(CF.GetInt(promotion.Start.Substring(0, 4)), CF.GetInt(promotion.Start.Substring(4, 2)), CF.GetInt(promotion.Start.Substring(6, 2)));
                DateTime end = new DateTime(CF.GetInt(promotion.End.Substring(0, 4)), CF.GetInt(promotion.End.Substring(4, 2)), CF.GetInt(promotion.End.Substring(6, 2)));

                if (start < DateTime.Now)
                    throw new Exception("Start Date have to greater than or equal Current Date");

                if (start >= end)
                    throw new Exception("Start Date have to smaller than End Date");

                // Check end date
                string query = @" SELECT MAX([End]) as MaxEndDate from Promotions where CategoryId = @CategoryId and BrandId = @BrandId ";
                var p = new { promotion.CategoryId, promotion.BrandId };
                dynamic result = await _dapperService.QueryFirstOrDefaultAsync(query, p);

                if (result.MaxEndDate != null)
                {
                    var maxEndDate = (DateTime)result.MaxEndDate;
                    if (start <= maxEndDate)
                        throw new Exception("There is a promotion for this product available ! Can not add more than 1 promotion in a time");
                }

                var newPromotion = new Promotion
                {
                    BrandId = promotion.BrandId,
                    CategoryId = promotion.CategoryId,
                    Start = start,
                    End = end,
                    PercentageDiscount = promotion.PercentageDiscount,
                };

                await _db.Promotions.AddAsync(newPromotion);
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task Update(int id, PromotionUpsertDTO promotion)
        {
            try
            {
                var existedPromotion = await _db.Promotions.FirstOrDefaultAsync(p => p.Id == id);
                if (existedPromotion == null) throw new Exception($"Promotion id {id} is not existed !");

                if (existedPromotion.CategoryId != promotion.CategoryId) existedPromotion.CategoryId = promotion.CategoryId;
                if (existedPromotion.BrandId != promotion.BrandId) existedPromotion.BrandId = promotion.BrandId;

                if (promotion.Start.Length != 8 || promotion.End.Length != 8)
                    throw new Exception("Date is invalid");

                existedPromotion.Start = new DateTime(CF.GetInt(promotion.Start.Substring(0, 4)), CF.GetInt(promotion.Start.Substring(4, 2)), CF.GetInt(promotion.Start.Substring(6, 2)));
                existedPromotion.End = new DateTime(CF.GetInt(promotion.End.Substring(0, 4)), CF.GetInt(promotion.End.Substring(4, 2)), CF.GetInt(promotion.End.Substring(6, 2)));

                if (existedPromotion.PercentageDiscount != promotion.PercentageDiscount) existedPromotion.PercentageDiscount = promotion.PercentageDiscount;

                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task Delete(int id)
        {
            try
            {
                var existedPromotion = await _db.Promotions.FirstOrDefaultAsync(p => p.Id == id);
                if (existedPromotion == null) throw new Exception($"Promotion id {id} is not existed !");

                _db.Promotions.Remove(existedPromotion);
                await _db.SaveChangesAsync();
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<List<PromotionDTO>> GetAll(string start, string end)
        {
            string query = @"SELECT
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

                            WHERE 1 = 1 --where
";
            string where = "";
            if (!string.IsNullOrEmpty(start))
                where += " AND promotion.Start >= CONVERT(DATETIME, @StartDate, 106) ";

            if (!string.IsNullOrEmpty(end))
                where += " AND promotion.[End] <= CONVERT(DATETIME, @EndDate, 106) ";

            query = query.Replace("--where", where);
            List<dynamic> result = await _dapperService.QueryAsync(query, new { StartDate = start, EndDate = end });
            if (result.Count == 0) return null;

            var promotions = new List<PromotionDTO>();

            foreach (var item in result)
            {
                var promotion = new PromotionDTO()
                {
                    Id = item.Id,
                    BrandId = item.BrandId,
                    BrandName = item.BrandName,
                    CategoryId = item.CategoryId,
                    CategoryName = item.CategoryName,
                    StartDate = item.StartDate,
                    EndDate = item.EndDate,
                    PercentageDiscount = item.PercentageDiscount,
                };

                promotions.Add(promotion);
            }

            return promotions;
        }

        public async Task<double> GetPercentageDiscount(int productId)
        {
            try
            {
                string sql = @" SELECT
	                                PercentageDiscount 

                                FROM Promotions promotion

                                INNER JOIN Brands brand ON brand.Id = promotion.BrandId
                                INNER JOIN Categories category ON category .Id = promotion.CategoryId
                                INNER JOIN Products product ON product.CategoryId = category.Id AND product.BrandId = brand.Id

                                WHERE promotion.Start <= GETDATE() AND promotion.[End] >= GETDATE() AND product.Id = @ProductId ";

                var p = new { ProductId = productId };
                dynamic result = await _dapperService.QueryFirstOrDefaultAsync(sql, p);
                if (result == null) return 0;
                return CF.GetDouble(result.PercentageDiscount);
            }
            catch (Exception ex)
            { throw new Exception(ex.Message); }
        }

    }
}
