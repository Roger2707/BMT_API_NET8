using Microsoft.Data.SqlClient;
using Store_API.Data;
using Store_API.DTOs;
using Store_API.DTOs.Baskets;
using Store_API.Extensions;
using Store_API.Helpers;
using Store_API.Models;

namespace Store_API.Repositories
{
    public class BasketRepository : IBasketRepository
    {
        private readonly IDapperService _dapperService;
        private readonly StoreContext _db;
        public BasketRepository(IDapperService dapperService, StoreContext db)
        {
            _dapperService = dapperService;
            _db = db;
        }
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

        public async Task<int> GetBasketIdByUsername(string username)
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

        public async Task HandleBasketMode(int userId, int productId, bool mode)
        {
            string sqlFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sql", "upsertbasket.sql");
            string query = System.IO.File.ReadAllText(sqlFilePath);
            var p = new { UserId = userId, ProductId = productId, Mode = mode };
            try
            {
                _dapperService.BeginTrans();
                await _dapperService.Execute(query, p);
                _dapperService.Commit();
            }
            catch (Exception ex)
            {
                _dapperService.Rollback();
                throw;
            }
        }

        public async void RemoveRange(List<BasketItemDTO> items)
        {
            var basketItems = new List<BasketItem>();

            foreach (var item in items)
            {
                var basketItem = await _db.BasketItems.FindAsync(item.BasketItemId);
                basketItems.Add(basketItem);
            }
            _db.BasketItems.RemoveRange(basketItems);
        }

        public async Task<Result<int>> ToggleStatusItems(string username, int itemId)
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
            try
            {
                _dapperService.BeginTrans();
                await _dapperService.Execute(query, p);
                _dapperService.Commit();

                return Result<int>.Success(1);
            }
            catch (Exception ex)
            {
                _dapperService.Rollback();
                return Result<int>.Failure("Change status failed !");
            }
        }

        public async Task<int> UpdateBasketPayment(string paymentIntentId, string clientSecret, string username)
        {
            int basketId = await GetBasketIdByUsername(username);
            if (basketId < 1) throw new Exception("Basket is not found !");

            string query = @"   Update Baskets 
                                SET PaymentIntentId = @PaymentIntentId, ClientSecret = @ClientSecret
                                WHERE Id = @Id  
                            ";

            var p = new { PaymentIntentId = paymentIntentId, ClientSecret = clientSecret, Id = basketId };

            try
            {
                _dapperService.BeginTrans();
                await _dapperService.Execute(query, p);
                _dapperService.Commit();

                return 1;
            }
            catch(SqlException ex)
            {
                _dapperService.Rollback();
                throw;
            }
        }
    }
}
