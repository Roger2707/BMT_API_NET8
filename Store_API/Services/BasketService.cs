using Microsoft.EntityFrameworkCore;
using Store_API.Data;
using Store_API.DTOs.Baskets;
using Store_API.Extensions;
using Store_API.Helpers;
using Store_API.Models;
using Store_API.Repositories;
using Stripe;
using System.Linq.Expressions;

namespace Store_API.Services
{
    public class BasketService : IBasketRepository
    {
        private readonly StoreContext _db;
        private readonly IDapperService _dapperService;
        public BasketService(StoreContext db, IDapperService dapperService)
        {
            _db = db;
            _dapperService = dapperService;  
        }

        #region GET 
        public async Task<BasketDTO> GetBasket(string currentUserLogin)
        {
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

            return result.MapBasket();
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

        public async Task UpdateBasketPayment(string paymentIntentId, string clientSecret, string username)
        {
            int basketId = await GetBasketIdByUsername(username);
            if (basketId == 0) throw new Exception("Basket is not found !");

            string query = @" Update Baskets 
                                SET PaymentIntentId = @PaymentIntentId, ClientSecret = @ClientSecret
                                WHERE Id = @Id  ";

            var p = new { PaymentIntentId = paymentIntentId, ClientSecret = clientSecret, Id = basketId };

            try
            {
                await _dapperService.Execute(query, p);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
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
