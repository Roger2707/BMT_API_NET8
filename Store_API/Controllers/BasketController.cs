using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store_API.DTOs.Baskets;
using Store_API.Helpers;
using Store_API.Services.IService;
using System.Security.Claims;

namespace Store_API.Controllers
{
    [Route("api/baskets")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        private readonly IBasketService _basketService;
        public BasketController(IBasketService basketService)
        {
            _basketService = basketService;
        }

        [HttpGet("get-basket")]
        [Authorize]
        public async Task<IActionResult> GetBasket()
        {
            int userId = CF.GetInt(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var redisBasket = await _basketService.GetBasketDTO(userId, User.Identity.Name);
            return Ok(redisBasket);
        }

        [Authorize]
        [HttpPost("upsert-basket")]
        public async Task<IActionResult> UpsertBasket([FromBody] BasketUpsertParam basketParams)
        {
            int userId = CF.GetInt(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var basketUpsertDTO = new BasketUpsertDTO
            {
                UserId = userId,
                Username = User.Identity.Name,
                ProductDetailId = basketParams.ProductDetailId,
                Quantity = basketParams.Quantity,
                Mode = basketParams.Mode
            };

            await _basketService.UpsertBasket(basketUpsertDTO);
            var basketDTO = await _basketService.GetBasketDTO(userId, User.Identity.Name);
            return Ok(basketDTO);
        }

        [Authorize]
        [HttpPost("toggle-status-item")]
        public async Task<IActionResult> ToggleItemsStatus(Guid itemId)
        {
            await _basketService.ToggleBasketItemStatus(User.Identity.Name, itemId);
            return Ok(itemId);
        }
    }
}
