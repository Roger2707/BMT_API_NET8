using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Store_API.DTOs.Baskets;
using Store_API.IService;
using Store_API.Models.Users;

namespace Store_API.Controllers
{
    [Route("api/baskets")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        private readonly IBasketService _basketService;
        private readonly IProductService _productService;
        private readonly UserManager<User> _userManager;
        public BasketController(IBasketService basketService, IProductService productService, UserManager<User> userManager)
        {
            _basketService = basketService;
            _productService = productService;
            _userManager = userManager;
        }

        [HttpGet("get-basket")]
        [Authorize]
        public async Task<IActionResult> GetBasket()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            int userId = user.Id;
            var redisBasket = await _basketService.GetBasketDTORedis(userId, User.Identity.Name);
            return Ok(redisBasket);
        }

        [Authorize]
        [HttpPost("upsert-basket")]
        public async Task<IActionResult> UpsertBasket([FromBody] BasketUpsertParam basketParams)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            int userId = user.Id;

            var basketUpsertDTO = new BasketUpsertDTO
            {
                UserId = userId,
                Username = User.Identity.Name,
                ProductDetailId = basketParams.ProductDetailId,
                Quantity = basketParams.Quantity,
                Mode = basketParams.Mode
            };
            var product = await _productService.GetProductSingleDetail(basketUpsertDTO.ProductDetailId);
            if (product == null) return BadRequest(new ProblemDetails { Title = $"Product Id: {basketUpsertDTO.ProductDetailId} not found !" });

            try
            {
                await _basketService.UpsertBasket(basketUpsertDTO);
                var redisBasket = await _basketService.GetBasketDTORedis(userId, User.Identity.Name);
                var newItemUpdated = redisBasket.Items.FirstOrDefault(i => i.ProductDetailId == basketUpsertDTO.ProductDetailId);
                return Ok(newItemUpdated);
            }
            catch (Exception ex)
            {
                return BadRequest(new ProblemDetails { Title = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("toggle-status-item")]
        public async Task<IActionResult> ToggleItemsStatus(Guid itemId)
        {
            try
            {
                await _basketService.ToggleBasketItemStatus(User.Identity.Name, itemId);
                return Ok(itemId);
            }
            catch (Exception ex)
            {
                return BadRequest(new ProblemDetails { Title = ex.Message });
            }       
        }
    }
}
