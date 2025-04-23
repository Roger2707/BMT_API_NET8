using Store_API.Data;
using Store_API.DTOs.Rating;
using Store_API.Models;

namespace Store_API.Repositories
{
    public class RatingRepository : Repository<Rating>, IRatingRepository
    {
        public RatingRepository(StoreContext db, IDapperService dapperService) : base(db, dapperService)
        {
        }

        #region GET
        public async Task<double> GetRatingProduct(Guid productId)
        {
            string query = @" SELECT AVG(CAST(Star AS FLOAT)) AS AverageRating FROM Ratings WHERE ProductId = @ProductId ";
            var star = await _dapperService.QueryFirstOrDefaultAsync<double>(query, new { ProductId = productId });
            return star;
        }

        public async Task<double> GetRatingProductDetail(Guid productDetailId)
        {
            string query = @" SELECT AVG(CAST(Star AS FLOAT)) AS AverageRating FROM Ratings WHERE ProductDetailId = @ProductDetailId ";
            var star = await _dapperService.QueryFirstOrDefaultAsync<double>(query, new { ProductDetailId = productDetailId });
            return star;
        }
        #endregion

        #region SET
        public async Task SetRating(RatingDTO ratingDTO)
        {
            string query = @" SELECT Id FROM Rating WHERE ProductId = @ProductId AND ProductDetailId = @ProductDetailId AND UserId = @UserId ";
            var ratingExisted = await _dapperService.QueryFirstOrDefaultAsync<Rating>(query, new { ratingDTO.ProductId, ratingDTO.ProductDetailId, ratingDTO.UserId });
            if (ratingExisted != null)
            {
                query = @" UPDATE Rating SET Star = @Star WHERE Id = @Id ";
                await _dapperService.Execute(query, new { ratingDTO.Star, Id = ratingExisted.Id });
            }
            else
            {
                var id = Guid.NewGuid();
                query = @" INSERT INTO Rating (Id, UserId, ProductId, ProductDetailId, Star) VALUES (@Id, @UserId, @ProductId, @ProductDetailId, @Star) ";
                await _dapperService.Execute(query, new { id = id, UserId = ratingDTO.UserId, ProductId = ratingDTO.ProductId, ProductDetailId = ratingDTO.ProductDetailId, Star = ratingDTO.Star });
            }
        }
        #endregion
    }
}
