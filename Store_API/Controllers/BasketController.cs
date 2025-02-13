using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using StackExchange.Redis;
using Store_API.DTOs.Baskets;
using Store_API.IService;
using Store_API.Models;
using Store_API.Repositories;
using System.Text.Json;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace Store_API.Controllers
{
    [Route("api/baskets")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDatabase _redis;
        private readonly UserManager<User> _userManager;
        private readonly IBasketService _basketService;
        public BasketController(IUnitOfWork unitOfWork, IConnectionMultiplexer redis, UserManager<User> userManager, IBasketService basketService)
        {
            _unitOfWork = unitOfWork;
            _redis = redis.GetDatabase();
            _userManager = userManager;
            _basketService = basketService;
        }

        [HttpGet("get-basket")]
        [Authorize]
        public async Task<IActionResult> GetBasket()
        {
            var basket = await _unitOfWork.Basket.GetBasket(User.Identity.Name);
            if(basket == null) return BadRequest(new ProblemDetails { Title = "Basket is empty now !" });
            return Ok(basket);
        }

        [Authorize]
        [HttpPost("upsert-basket")]
        public async Task<IActionResult> UpsertBasket(int productId, int mode)
        {
            var product = await _unitOfWork.Product.GetById(productId);
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
            var product = await _unitOfWork.Product.GetById(itemId);
            if (product == null) return BadRequest(new ProblemDetails { Title = $"Product Id: {itemId} not found !" });

            var result = await _unitOfWork.Basket.ToggleStatusItems(User.Identity.Name, itemId);
            if (!result.IsSuccess) return BadRequest(new ProblemDetails { Title = result.Errors[0] });
            return Ok(result);
        }
    }
}
