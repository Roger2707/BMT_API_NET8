using Store_API.DTOs.Rating;
using Store_API.Models;

namespace Store_API.Repositories
{
    public interface IRatingRepository : IRepository<Rating>
    {
        Task SetRating(RatingDTO ratingDTO);
        Task<double> GetRatingProduct(Guid productId);
        Task<double> GetRatingProductDetail(Guid productDetailId);
    }
}
