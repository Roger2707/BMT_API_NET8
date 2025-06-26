using Microsoft.AspNetCore.Mvc;
using Store_API.DTOs.Promotions;
using Store_API.Models;
using Store_API.Services.IService;

namespace Store_API.Controllers
{
    [Route("api/promotions")]
    [ApiController]
    public class PromotionsController : ControllerBase
    {
        private readonly IPromotionService _promotionService;
        public PromotionsController(IPromotionService promotionService)
        {
            _promotionService = promotionService;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {
            var promotions = await _promotionService.GetAll();
            if (promotions == null || promotions.Count <= 0)  return BadRequest(new ProblemDetails { Title = "Promotions are empty now !" });
            return Ok(promotions);
        }

        [HttpGet("get-promotion-detail", Name = "GetDetailPromotion")]
        public async Task<IActionResult> GetDetailPromotion(Guid promotionId)
        {
            var promotion = await _promotionService.GetPromotion(promotionId);
            if (promotion == null) return BadRequest(new ProblemDetails { Title = "Promotion is not Existed !" });
            return Ok(promotion);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromForm] PromotionUpsertDTO promotion)
        {
            try
            {
                await _promotionService.Create(promotion);
                var newPromotion = await _promotionService.GetPromotion(promotion.Id);
                return CreatedAtRoute("GetDetailPromotion", new { promotionId = promotion.Id }, newPromotion);
            }
            catch (Exception ex)
            {
                return BadRequest(new ProblemDetails { Title = ex.Message });
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update([FromForm] PromotionUpsertDTO promotion)
        {
            if(promotion.Id == Guid.Empty) 
                return BadRequest(new ProblemDetails { Title = "Id can not be empty !" });

            try
            {
                await _promotionService.Update(promotion);
                var promotionUpdated = await _promotionService.GetPromotion(promotion.Id);
                return CreatedAtRoute("GetDetailPromotion", new { promotionId = promotion.Id }, promotionUpdated);
            }
            catch (Exception ex)
            {
                return BadRequest(new ProblemDetails { Title = ex.Message });
            }
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest(new ProblemDetails { Title = "Id can not be empty !" });
            try
            {
                await _promotionService.Delete(id);
                return Ok(1);
            }
            catch (Exception ex)
            {
                return BadRequest(new ProblemDetails { Title = ex.Message });
            }
        }
    }
}
