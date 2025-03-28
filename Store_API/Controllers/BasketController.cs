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
            var basket = await _basketService.GetBasket(User.Identity.Name);
            return Ok(basket);
        }

        [Authorize]
        [HttpPost("upsert-basket")]
        public async Task<IActionResult> UpsertBasket(Guid productId, int mode)
        {
            var product = await _unitOfWork.Product.GetProductDTODetail(productId);
            if (product == null) return BadRequest(new ProblemDetails { Title = $"Product Id: {productId} not found !" });

            int userId = (await _userManager.FindByNameAsync(User.Identity.Name)).Id;
            bool modeParam = mode == 1 ? true : false;
            
            var result = await _basketService.HandleBasketMode(userId, productId, modeParam, User.Identity.Name);
            if(!result.IsSuccess) return BadRequest(new ProblemDetails { Title = result.Errors[0] }); 
            return Ok(result);
        }

        [Authorize]
        [HttpPost("toggle-status-item")]
        public async Task<IActionResult> ToggleItemsStatus(int itemId)
        {
            var result = await _basketService.ToggleStatusItems(User.Identity.Name, itemId);
            if (!result.IsSuccess) return BadRequest(new ProblemDetails { Title = result.Errors[0] });
            return Ok(result);
        }
    }
}
