using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Store_API.IService;
using Store_API.Models.Users;
using Store_API.Repositories;

namespace Store_API.Controllers
{
    [Route("api/baskets")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly IBasketService _basketService;
        public BasketController(IUnitOfWork unitOfWork, UserManager<User> userManager, IBasketService basketService)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _basketService = basketService;
        }

        [HttpGet("get-basket")]
        [Authorize]
        public async Task<IActionResult> GetBasket()
        {
            return Ok();
        }

        [Authorize]
        [HttpPost("upsert-basket")]
        public async Task<IActionResult> UpsertBasket(Guid productId, int mode)
        {
            var product = await _unitOfWork.Product.GetProductDTO(productId);
            if (product == null) return BadRequest(new ProblemDetails { Title = $"Product Id: {productId} not found !" });

            int userId = (await _userManager.FindByNameAsync(User.Identity.Name)).Id;
            bool modeParam = mode == 1 ? true : false;
            

            return Ok();
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
