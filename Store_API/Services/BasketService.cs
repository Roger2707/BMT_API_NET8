using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using Store_API.Data;
using Store_API.DTOs;
using Store_API.DTOs.Baskets;
using Store_API.Extensions;
using Store_API.Helpers;
using Store_API.IService;
using Store_API.Models;
using Store_API.Repositories;
using Stripe;
using System.Linq.Expressions;
using System.Text.Json;

namespace Store_API.Services
{
    public class BasketService : IBasketService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDatabase _redis;
        public BasketService(IUnitOfWork unitOfWork, IConnectionMultiplexer redis)
        {
            _unitOfWork = unitOfWork;
            _redis = redis.GetDatabase();
        }

        #region GET 
        public async Task<BasketDTO> GetBasket(string currentUserLogin)
        {
            // Get Key Redis
            string basketKey = $"basket:{currentUserLogin}";

            // Get basket existed in redis
            var cacheBasket = await _redis.StringGetAsync(basketKey);

            // If basket existed in redis, return it to controller
            if (!string.IsNullOrEmpty(cacheBasket))
            {
                var cartFromCache = JsonSerializer.Deserialize<BasketDTO>(cacheBasket);
                return cartFromCache;
            }

            // If not => Retrieve DB
            var basketDTO = await _unitOfWork.Basket.GetBasket(currentUserLogin);

            // Store the basket in Redis cache - Set up Time Life
            var serializedCart = JsonSerializer.Serialize(basketDTO);
            await _redis.StringSetAsync(basketKey, serializedCart, TimeSpan.FromMinutes(30));

            return basketDTO;
        }

        #endregion

        #region Actions
        public async Task<Result<BasketDTO>> HandleBasketMode(int userId, int productId, bool mode, string currentUserLogin)
        {
            BasketDTO basketDTO = null;
            string basketKey = $"basket:{currentUserLogin}";
            try
            {
                await _unitOfWork.Basket.HandleBasketMode(userId, productId, mode);
                basketDTO = await GetBasket(currentUserLogin);
            }
            catch (Exception ex)
            {
                await _redis.KeyDeleteAsync(basketKey);
                return Result<BasketDTO>.Failure(ex.Message);
            }
            return Result<BasketDTO>.Success(basketDTO);
        }

        public async Task<Result<BasketDTO>> UpdateBasketPayment(string paymentIntentId, string clientSecret, string username)
        {
            BasketDTO basketDTO = null;
            string basketKey = $"basket:{username}";
            try
            {
                await _unitOfWork.Basket.UpdateBasketPayment(paymentIntentId, clientSecret, username);
                basketDTO = await GetBasket(username);
            }
            catch (Exception ex)
            {
                await _redis.KeyDeleteAsync(basketKey);
                return Result<BasketDTO>.Failure(ex.Message);
            }
            return Result<BasketDTO>.Success(basketDTO);
        }

        #endregion

    }
}
