using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> UpsertBasket(int productId)
        {
            var product = await _unitOfWork.Product.GetById(productId);
            if (product == null) return BadRequest(new ProblemDetails { Title = $"Product Id: {productId} not found !" });
            string error = "";
            int userId = (await _userManager.FindByNameAsync(User.Identity.Name)).Id;
            string basketKey = $"basket:{User.Identity.Name}";
            BasketDTO basketDb = null;

            try
            {
                _unitOfWork.BeginTrans();

                // 1. Upsert in Database
                await _unitOfWork.Basket.HandleBasketMode(userId, productId, true);

                // 2. Sync in Redis
                basketDb = await _unitOfWork.Basket.GetBasket(User.Identity.Name);
                var serializedCart = JsonSerializer.Serialize(basketDb);
                await _redis.StringSetAsync(basketKey, serializedCart, TimeSpan.FromMinutes(30));

                _unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                error = ex.Message;

                // 1. Rollback
                _unitOfWork.Rollback();

                // 2. Remove Key Redis (Cache Invalidation)
                await _redis.KeyDeleteAsync(basketKey);
            }
            finally
            {
                _unitOfWork.CloseConnection();
            }

            if (error != "") return BadRequest(new ProblemDetails { Title = error });
            return CreatedAtRoute("get-basket", basketDb);
        }

        [Authorize]
        [HttpPost("toggle-status-item")]
        public async Task<IActionResult> ToggleItemsStatus(int itemId)
        {
            string error = "";
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
                error = ex.Message;

                // 1. Rollback
                _unitOfWork.Rollback();

                // 2. Remove Key Redis (Cache Invalidation)
                await _redis.KeyDeleteAsync(basketKey);
            }
            finally
            {
                _unitOfWork.CloseConnection();
            }

            if (error != "") return BadRequest(new ProblemDetails { Title = error });
            return CreatedAtRoute("get-basket", basketDb);
        }
    }
}
