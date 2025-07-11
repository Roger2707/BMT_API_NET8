﻿using Store_API.DTOs.Rating;

namespace Store_API.Services.IService
{
    public interface IRatingService
    {
        Task<double> GetProductRating(Guid productId);
        Task<double> GetProductDetailRating(Guid productDetailId);
        Task SetRating(RatingDTO ratingDTO);
    }
}
