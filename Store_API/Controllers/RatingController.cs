using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store_API.DTOs.Rating;
using Store_API.Helpers;
using Store_API.Services.IService;
using System.Security.Claims;

namespace Store_API.Controllers
{
    [Route("api/ratings")]
    [ApiController]
    [Authorize]
    public class RatingController : ControllerBase
    {
        private readonly IRatingService _ratingService;
        public RatingController(IRatingService ratingService)
        {
            _ratingService = ratingService;
        }

        [HttpGet("get-product-rating")]
        public async Task<IActionResult> GetProductRate(Guid productId)
        {
            var star = await _ratingService.GetProductRating(productId);
            return Ok(star);
        }

        [HttpGet("get-product-detail-rating")]
        public async Task<IActionResult> GetProductDetailRate(Guid productDetailId)
        {
            var star = await _ratingService.GetProductDetailRating(productDetailId);
            return Ok(star);
        }

        [HttpPost("set-rating")]
        public async Task<IActionResult> SetRating([FromBody] RatingDTO ratingDTO)
        {
            int userId = CF.GetInt(User.FindFirstValue(ClaimTypes.NameIdentifier));
            ratingDTO.UserId = userId;

            await _ratingService.SetRating(ratingDTO);
            return Ok(new { Title = $"Set Rating: {ratingDTO.ProductDetailId} Successfully !" });
        }
    }
}
