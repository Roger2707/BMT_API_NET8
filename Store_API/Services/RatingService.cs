using Store_API.DTOs.Rating;
using Store_API.IService;
using Store_API.Repositories;

namespace Store_API.Services
{
    public class RatingService : IRatingService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RatingService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<double> GetProductDetailRating(Guid productDetailId)
        {
            return await _unitOfWork.Rating.GetRatingProductDetail(productDetailId);  
        }

        public async Task<double> GetProductRating(Guid productId)
        {
            return await _unitOfWork.Rating.GetRatingProduct(productId);
        }

        public async Task SetRating(RatingDTO ratingDTO)
        {
            if (ratingDTO.Star < 0.5) return;
            await _unitOfWork.BeginTransactionAsync(Enums.TransactionType.Dapper);
            try
            {
                await _unitOfWork.Rating.SetRating(ratingDTO);
                await _unitOfWork.CommitAsync();
            }
            catch(Exception ex)
            {
                await _unitOfWork.RollbackAsync();
            }
        }
    }
}
