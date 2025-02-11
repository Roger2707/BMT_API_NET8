using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using Store_API.Data;
using Store_API.DTOs.Baskets;
using Store_API.Extensions;
using Store_API.Helpers;
using Store_API.Models;
using Store_API.Repositories;
using Stripe;
using System.Linq.Expressions;
using System.Text.Json;

namespace Store_API.Services
{
    public class BasketService : IBasketRepository
    {
        private readonly StoreContext _db;
        private readonly IDapperService _dapperService;
        private readonly IDatabase _redis;
        public BasketService(StoreContext db, IDapperService dapperService, IConnectionMultiplexer redis)
        {
            _db = db;
            _dapperService = dapperService;  
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

            string query = @"
                            SELECT 

                            basket.Id
                            , basket.UserId
                            , items.Id as BasketItemId
                            , items.ProductId
                            , product.Name as ProductName
                            , IIF(product.ImageUrl IS NOT NULL, 
                                (SELECT TOP 1 value 
                                 FROM STRING_SPLIT(product.ImageUrl, ',')), 
                                '') AS ProductFirstImage
                            , items.Quantity
                            , product.Price as OriginPrice
                            , ISNULL(promotion.PercentageDiscount, 0) as DiscountPercent
                            , IIF(promotion.PercentageDiscount is not NULL, product.Price - (product.Price * (promotion.PercentageDiscount / 100)), product.Price) as DiscountPrice
                            , items.Status
                            , basket.PaymentIntentId
                            , basket.ClientSecret

                            FROM Baskets basket

                            INNER JOIN BasketItems items ON items.BasketId = basket.Id
                            INNER JOIN Products product ON product.Id = items.ProductId
                            INNER JOIN Brands brand ON brand.Id = product.BrandId
                            INNER JOIN Categories category ON category.Id = product.CategoryId
                            LEFT JOIN Promotions promotion 
                            ON promotion.CategoryId = category.Id AND promotion.BrandId = brand.Id AND GETDATE() <= promotion.[End]
                            INNER JOIN AspNetUsers u ON u.Id = basket.UserId

                            WHERE u.UserName = @UserName

                            ";

            var p = new { UserName = currentUserLogin };
            List<dynamic> result = await _dapperService.QueryAsync(query, p);

            if (result == null || result.Count <= 0) return null;

            var basketDTO = result.MapBasket();

            // Store the basket in Redis cache - Set up Time Life
            var serializedCart = JsonSerializer.Serialize(basketDTO);
            await _redis.StringSetAsync(basketKey, serializedCart, TimeSpan.FromMinutes(30));

            return basketDTO;
        }

        private async Task<int> GetBasketIdByUsername(string username)
        {
            string query = @" SELECT b.Id 
                              FROM Baskets b
                              INNER JOIN AspNetUsers u ON u.Id = b.UserId
                              WHERE u.UserName = @UserName
                            ";

            var p = new { UserName = username };
            int basketId = CF.GetInt((await _dapperService.QueryFirstOrDefaultAsync(query, p)).Id);
            return basketId;
        }

        #endregion

        #region Actions
        public async Task HandleBasketMode(int userId, int productId, bool mode)
        {
            string sqlFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sql", "upsertbasket.sql");
            string query = System.IO.File.ReadAllText(sqlFilePath);
            var p = new { UserId = userId, ProductId = productId, Mode = mode };
            await _dapperService.Execute(query, p);
        }

        public async Task<BasketDTO> UpdateBasketPayment(string paymentIntentId, string clientSecret, string username)
        {
            int basketId = await GetBasketIdByUsername(username);
            if (basketId < 1) throw new Exception("Basket is not found !");

            string query = @"   Update Baskets 
                                SET PaymentIntentId = @PaymentIntentId, ClientSecret = @ClientSecret
                                WHERE Id = @Id  
                            ";

            var p = new { PaymentIntentId = paymentIntentId, ClientSecret = clientSecret, Id = basketId };

            BasketDTO basketDTO = null;
            string basketKey = $"basket:{username}";
            try
            {
                await _dapperService.Execute(query, p);
                // Get basket updated (redis is synced)
                basketDTO = await GetBasket(username);
            }
            catch (Exception ex)
            {
                await _redis.KeyDeleteAsync(basketKey);
                throw new Exception(ex.Message);
            }
            return basketDTO;
        }

        public async Task ToggleStatusItems(string username, int itemId)
        {
            try
            {
                var basket = await GetBasket(username);
                string query = @"
                                DECLARE @CurrentStatus BIT
                                SELECT @CurrentStatus = Status FROM BasketItems WHERE BasketId = @BasketId AND Id = @BasketItemsId
                                IF(@CurrentStatus = 0)
                                    UPDATE BasketItems SET Status = 1 WHERE BasketId = @BasketId AND Id = @BasketItemsId
                                ELSE
                                    UPDATE BasketItems SET Status = 0 WHERE BasketId = @BasketId AND Id = @BasketItemsId
                                ";

                var p = new { BasketId = basket.Id, BasketItemsId = itemId };

                await _dapperService.Execute(query, p);
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message}");
            }
        }

        #endregion

    }
}
