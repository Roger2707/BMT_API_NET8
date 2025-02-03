using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using StackExchange.Redis;
using Store_API.DTOs.Baskets;
using Store_API.Models;
using Store_API.Repositories;
using System.Text.Json;

namespace Store_API.Controllers
{
    [Route("api/baskets")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDatabase _redis;
        private readonly UserManager<User> _userManager;
        public BasketController(IUnitOfWork unitOfWork, IConnectionMultiplexer redis, UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _redis = redis.GetDatabase();
            _userManager = userManager;
        }

        [HttpGet("get-basket")]
        [Authorize]
        public async Task<IActionResult> GetBasket()
        {
            string basketKey = $"basket:{User.Identity.Name}";

            // Get basket existed in redis
            var cacheBasket = await _redis.StringGetAsync(basketKey);

            // If basket existed in redis, return it
            if (!string.IsNullOrEmpty(cacheBasket))
            {
                var cartFromCache = JsonSerializer.Deserialize<BasketDTO>(cacheBasket);
                return Ok(cartFromCache);
            }

            // If basket not existed in redis, get it from database
            var basketDb = await _unitOfWork.Basket.GetBasket(User.Identity.Name);

            // If basket not existed in database, return not found
            if (basketDb == null) return NotFound();

            // Store the basket in Redis cache
            var serializedCart = JsonSerializer.Serialize(basketDb);
            await _redis.StringSetAsync(basketKey, serializedCart, TimeSpan.FromMinutes(30));
            return Ok(basketDb);
        }

        [Authorize]
        [HttpPost("upsert-basket")]
        public async Task<IActionResult> UpsertBasket(int productId, int mode)
        {
            var product = await _unitOfWork.Product.GetById(productId);
            if (product == null) return BadRequest(new ProblemDetails { Title = $"Product Id: {productId} not found !" });

            int userId = (await _userManager.FindByNameAsync(User.Identity.Name)).Id;
            string basketKey = $"basket:{User.Identity.Name}";
            BasketDTO basketDb = null;

            try
            {
                // 1. Upsert in Database
                bool modeParam = mode == 1 ? true : false;
                await _unitOfWork.Basket.HandleBasketMode(userId, productId, modeParam);

                // 2. Sync in Redis
                basketDb = await _unitOfWork.Basket.GetBasket(User.Identity.Name);
                var serializedCart = JsonSerializer.Serialize(basketDb);
                await _redis.StringSetAsync(basketKey, serializedCart, TimeSpan.FromMinutes(30));
            }
            catch (SqlException ex)
            {
                // Remove Key Redis (Cache Invalidation)
                await _redis.KeyDeleteAsync(basketKey);
                return BadRequest(new ProblemDetails { Title = ex.Message });
            }
            return Ok(basketDb);
        }

        [Authorize]
        [HttpPost("toggle-status-item")]
        public async Task<IActionResult> ToggleItemsStatus(int itemId)
        {
            BasketDTO basketDb = null;
            string basketKey = $"basket:{User.Identity.Name}";

            try
            {
                _unitOfWork.BeginTrans();

                // 1. Upsert in Database
                await _unitOfWork.Basket.ToggleStatusItems(User.Identity.Name, itemId);

                // 2. Sync in Redis
                basketDb = await _unitOfWork.Basket.GetBasket(User.Identity.Name);
                var serializedCart = JsonSerializer.Serialize(basketDb);
                await _redis.StringSetAsync(basketKey, serializedCart, TimeSpan.FromMinutes(30));

                _unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                // 1. Rollback
                _unitOfWork.Rollback();

                // 2. Remove Key Redis (Cache Invalidation)
                await _redis.KeyDeleteAsync(basketKey);

                return BadRequest(new ProblemDetails { Title = ex.Message });
            }
            return Ok(basketDb);
        }
    }
}
