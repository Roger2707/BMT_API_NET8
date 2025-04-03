using Microsoft.Data.SqlClient;
using StackExchange.Redis;
using Store_API.Data;
using Store_API.DTOs.Baskets;
using Store_API.Extensions;
using System.Text.Json;

namespace Store_API.Repositories
{
    public class BasketRepository : IBasketRepository
    {
        private readonly IDapperService _dapperService;
        private readonly IDatabase _redis;
        public BasketRepository(IDapperService dapperService, IConnectionMultiplexer redis)
        {
            _dapperService = dapperService;
            _redis = redis.GetDatabase();
        }

        #region Basic CRUD

        public async Task<bool> CheckBasketExistedDB(int userId, Guid basketId)
        {
            string query = @" SELECT COUNT(1) FROM Baskets WHERE Id = @BasketId AND UserId = @UserId ";
            var p = new { UserId = userId, BasketId = basketId };
            int result = await _dapperService.QueryFirstOrDefaultAsync<int>(query, p);
            return result > 0;
        }

        public async Task DeleteBasket(int userId, Guid basketId)
        {
            string query = @" DELETE Baskets WHERE Id = @BasketId AND UserId = @UserId ";
            var p = new { BasketId = basketId, UserId = userId };
            await _dapperService.Execute(query, p);
        }

        public async Task DeleteBasketItem(Guid basketId)
        {
            string query = @" DELETE BasketItems WHERE BasketId = @BasketId ";
            var p = new { BasketId = basketId };
            await _dapperService.Execute(query, p);
        }

        public async Task InsertBasket(BasketDTO basketDTO)
        {
            string query = @" INSERT INTO Baskets (Id, UserId) VALUES (@Id, @UserId) ";
            var p = new { Id = basketDTO.Id, UserId = basketDTO.UserId };
            await _dapperService.Execute(query, p);
        }

        public async Task InsertBasketItems(BasketDTO basketDTO)
        {
            if (basketDTO.Items == null || !basketDTO.Items.Any()) return;

            string query = @"   INSERT INTO BasketItems(Id, BasketId, ProductDetailId, Quantity, Status) 
                                VALUES(@Id, @BasketId, @ProductDetailId, @Quantity, @Status) ";

            var parameters = basketDTO.Items.Select(item => new
            {
                Id = item.BasketItemId,
                BasketId = basketDTO.Id,
                ProductDetailId = item.ProductDetailId,
                Quantity = item.Quantity,
                Status = item.Status
            }).ToList();

            await _dapperService.Execute(query, parameters);
        }

        #endregion

        #region Retrieve 

        public async Task<BasketDTO> GetBasketDTORedis(int userId, string username)
        {
            string basketKey = $"basket:{username}";
            var redisBasket = await _redis.StringGetAsync(basketKey);

            if (string.IsNullOrEmpty(redisBasket))
            {
                var basketDB = await GetBasketDTODB(username);

                if (basketDB == null)
                    return new BasketDTO { Items = new List<BasketItemDTO>(), UserId = userId, Id = Guid.NewGuid(), GrandTotal = 0 };

                var basketJson = JsonSerializer.Serialize(basketDB);
                await _redis.StringSetAsync(basketKey, basketJson, TimeSpan.FromMinutes(30));
                return basketDB;
            }
            var basketDTO = JsonSerializer.Deserialize<BasketDTO>(redisBasket);
            return basketDTO;
        }

        public async Task<BasketDTO> GetBasketDTODB(string username)
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
                            --, items.Quantity
                            --, product.Price as OriginPrice
                            --, ISNULL(promotion.PercentageDiscount, 0) as DiscountPercent
                            --, IIF(promotion.PercentageDiscount is not NULL, product.Price - (product.Price * (promotion.PercentageDiscount / 100)), product.Price) as DiscountPrice
                            , items.Status

                            FROM Baskets basket

                            INNER JOIN BasketItems items ON items.BasketId = basket.Id
                            INNER JOIN Products product ON product.Id = items.ProductId
                            INNER JOIN Brands brand ON brand.Id = product.BrandId
                            INNER JOIN Categories category ON category.Id = product.CategoryId
                            LEFT JOIN Promotions promotion 
                                ON promotion.CategoryId = category.Id AND promotion.BrandId = brand.Id AND GETDATE() <= promotion.EndDate
                            INNER JOIN AspNetUsers u ON u.Id = basket.UserId

                            WHERE u.UserName = @UserName

                            ";

            var p = new { UserName = username };
            var result = await _dapperService.QueryAsync<BasketDapperRow>(query, p);
            if (result == null || result.Count <= 0) return null;

            return result.MapBasket();
        }

        #endregion

        //
        #region Basket Payment ( will update later )

        public async Task<int> UpdateBasketPayment(string paymentIntentId, string clientSecret, string username)
        {
            string query = @"                               

                                -- Update later - get basket Id - combine in one function 

                              SELECT b.Id 
                              FROM Baskets b
                              INNER JOIN AspNetUsers u ON u.Id = b.UserId
                              WHERE u.UserName = @UserName

                              Update Baskets 
                              SET PaymentIntentId = @PaymentIntentId, ClientSecret = @ClientSecret
                              WHERE Id = @Id  
                            ";

            var p = new { PaymentIntentId = paymentIntentId, ClientSecret = clientSecret };

            try
            {
                await _dapperService.BeginTransactionAsync();
                await _dapperService.Execute(query, p);
                await _dapperService.CommitAsync();

                return 1;
            }
            catch (SqlException ex)
            {
                await _dapperService.RollbackAsync();
                throw;
            }
        }

        #endregion
    }
}
