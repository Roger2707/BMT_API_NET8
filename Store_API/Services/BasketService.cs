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
                                , items.Quantity
                                , items.Price as OriginPrice
                                , ISNULL(promotion.PercentageDiscount, 0) as Discount
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
        public async Task AddItem(string username, int productId, int quantity)
        {
            int basketId = await GetBasketIdByUsername(username);

            // Check Item existed ?
            int? oldQuantity = (await _db.BasketItems.Where(x => x.BasketId == basketId && x.ProductId == productId).FirstOrDefaultAsync())?.Quantity;

            // Find Detail Product
            var product = await _db.Products.FindAsync(productId);

            // Query
            string query = "";
            var p = new object { };

            if (CF.GetInt(oldQuantity) > 0) // Update Quantity and Price
            {
                int updatedQuantity = (int)oldQuantity + quantity;
                double price = product.Price * updatedQuantity;

                query = " UPDATE BasketItems SET Quantity = @Quantity, Price = @Price WHERE BasketId = @BasketId AND ProductId = @ProductId ";
                p = new { Quantity = updatedQuantity, Price = price, BasketId = basketId, ProductId = productId };
            }
            else // Create
            {
                double price = product.Price * quantity;

                query = @" INSERT INTO BasketItems (BasketId, ProductId, Quantity, Price) VALUES (@BasketId, @ProductId, @Quantity, @Price) ";
                p = new { BasketId = basketId, ProductId = productId, Quantity = quantity, Price = price };
            }

            try
            {
                await _dapperService.Execute(query, p);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task RemoveItem(string username, int productId, int quantity)
        {
            int basketId = await GetBasketIdByUsername(username);

            // Check Item existed ?
            int? oldQuantity = (await _db.BasketItems.Where(x => x.BasketId == basketId && x.ProductId == productId).FirstOrDefaultAsync())?.Quantity;

            // Find Detail Product
            var product = await _db.Products.FindAsync(productId);

            // Get Quantity after remove
            int updatedQuantity = (int)oldQuantity - quantity;

            // Query
            string query = "";
            var p = new object { };

            if (CF.GetInt(updatedQuantity) > 0) // Update Quantity and Price
            {
                double price = product.Price * updatedQuantity;

                query = " UPDATE BasketItems SET Quantity = @Quantity, Price = @Price WHERE BasketId = @BasketId AND ProductId = @ProductId ";
                p = new { Quantity = updatedQuantity, Price = price, BasketId = basketId, ProductId = productId };
            }
            else // Delete
            {
                query = @" DELETE BasketItems WHERE BasketId = @BasketId AND ProductId = @ProductId ";
                p = new { BasketId = basketId, ProductId = productId };
            }

            try
            {
                await _dapperService.Execute(query, p);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
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

        #region Helper
        public async Task<double> GetPercentageDiscount(int productId)
        {
            try
            {
                string sql = @" SELECT
	                                PercentageDiscount 

                                FROM Promotions promotion

                                INNER JOIN Brands brand ON brand.Id = promotion.BrandId
                                INNER JOIN Categories category ON category .Id = promotion.CategoryId
                                INNER JOIN Products product ON product.CategoryId = category.Id AND product.BrandId = brand.Id

                                WHERE promotion.Start <= GETDATE() AND promotion.[End] >= GETDATE() AND product.Id = @ProductId ";

                var p = new { ProductId = productId };
                dynamic result = await _dapperService.QueryFirstOrDefaultAsync(sql, p);
                if (result == null) return 0;
                return CF.GetDouble(result.PercentageDiscount);
            }
            catch (Exception ex)
            { throw new Exception(ex.Message); }
        }

        #endregion

    }
}
