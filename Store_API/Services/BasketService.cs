using Store_API.DTOs.Baskets;
using Store_API.Enums;
using Store_API.Infrastructures;
using Store_API.Services.IService;

namespace Store_API.Services
{
    public class BasketService : IBasketService
    {
        private readonly IRedisService _redisService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProductService _productService;

        public BasketService(
            IRedisService redisService, 
            IUnitOfWork unitOfWork, 
            IProductService productService)
        {
            _redisService = redisService;
            _unitOfWork = unitOfWork;
            _productService = productService;
        }

        #region Retrieve Data

        public async Task<BasketDTO> GetBasketDTO(int userId, string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username cannot be empty", nameof(username));

            var basketCache = await _unitOfWork.Basket.GetBasket(username);
            return basketCache;
        }

        #endregion

        #region Sync database Implementations

        public async Task<IEnumerable<string>> GetBasketKeysAsync()
        {
            return await _redisService.GetKeysAsync("basket:*");
        }

        public async Task<TimeSpan?> GetBasketTTLAsync(string key)
        {
            return await _redisService.GetKeyTimeToLiveAsync(key);
        }

        public async Task SyncBasketDB(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username cannot be empty", nameof(username));

            string basketKey = $"basket:{username}";
            var redisBasket = await _redisService.GetAsync<BasketDTO>(basketKey);
            if (redisBasket == null) return;

            try
            {
                await _unitOfWork.BeginTransactionAsync(TransactionType.Dapper);

                await _unitOfWork.Basket.DeleteBasketItem(redisBasket.Id);
                await _unitOfWork.Basket.DeleteBasket(redisBasket.UserId, redisBasket.Id);

                // Only sync if basket has items
                if (redisBasket.Items.Any())
                {
                    await _unitOfWork.Basket.InsertBasket(redisBasket);
                    await _unitOfWork.Basket.InsertBasketItems(redisBasket);
                }

                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw new Exception("An error occurred while syncing the basket.", ex);
            }
        }

        #endregion

        #region CRUD Operations (Implement on Redis Server)

        public async Task UpsertBasket(BasketUpsertDTO basketUpsertDTO)
        {
            #region Validations

            if (basketUpsertDTO == null)
                throw new ArgumentNullException(nameof(basketUpsertDTO));

            if (string.IsNullOrWhiteSpace(basketUpsertDTO.Username))
                throw new ArgumentException("Username cannot be empty", nameof(basketUpsertDTO.Username));

            var productDetail = await _unitOfWork.ProductDetail.FindFirstAsync(x => x.Id == basketUpsertDTO.ProductDetailId);
            if (productDetail == null) 
                throw new ArgumentException($"Product Id: {basketUpsertDTO.ProductDetailId} not found !");

            if (basketUpsertDTO.Quantity <= 0)
                throw new ArgumentException("Quantity must be greater than 0", nameof(basketUpsertDTO.Quantity));

            #endregion

            string basketKey = $"basket:{basketUpsertDTO.Username}";
            try
            {
                var basket = await _unitOfWork.Basket.GetBasket(basketUpsertDTO.Username);
                if(basket == null) basket = new BasketDTO
                {
                    Id = Guid.NewGuid(),
                    UserId = basketUpsertDTO.UserId,
                    Items = new List<BasketItemDTO>()
                };

                await UpdateBasketItems(basket, basketUpsertDTO, productDetail.ProductId);
                await _redisService.SetAsync<BasketDTO>(basketKey, basket, TimeSpan.FromMinutes(1));
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to upsert basket for user {basketUpsertDTO.Username}", ex);
            }
        }

        private async Task UpdateBasketItems(BasketDTO basket, BasketUpsertDTO basketUpsertDTO, Guid productId)
        {
            var existingItem = basket.Items.FirstOrDefault(item => item.ProductDetailId == basketUpsertDTO.ProductDetailId);
            var productDTO = await _productService.GetProductDTO(productId);
            var productDetailDTO = productDTO.Details.FirstOrDefault(detail => detail.Id == basketUpsertDTO.ProductDetailId);           

            // If the existedItem is null - will care if mode Add . Remove Mode do nothing.
            if (existingItem == null)
            {
                if(basketUpsertDTO.Mode == BasketMode.Add)
                {
                    basket.Items.Add(new BasketItemDTO
                    {
                        BasketItemId = Guid.NewGuid(),
                        ProductDetailId = productDetailDTO.Id,
                        Quantity = basketUpsertDTO.Quantity,
                        Status = true,
                        ProductName = productDTO.Name,
                        ProductFirstImage = productDetailDTO.ImageUrl.Split(',')[0],
                        OriginPrice = productDetailDTO.OriginPrice,
                        DiscountPercent = productDetailDTO.PercentageDiscount,
                        DiscountPrice = productDetailDTO.DiscountPrice
                    });
                }
                return;
            }

            switch (basketUpsertDTO.Mode)
            {
                case BasketMode.Add:
                    existingItem.Quantity += basketUpsertDTO.Quantity;
                    break;
                case BasketMode.Remove:
                    existingItem.Quantity -= basketUpsertDTO.Quantity;
                    if (existingItem.Quantity <= 0)
                    {
                        basket.Items.Remove(existingItem);
                    }
                    break;
                default:
                    throw new ArgumentException($"Invalid basket mode: {basketUpsertDTO.Mode}");
            }
        }

        public async Task ToggleBasketItemStatus(string username, Guid basketItemId)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username cannot be empty", nameof(username));

            if (basketItemId == Guid.Empty)
                throw new ArgumentException("Basket item ID cannot be empty", nameof(basketItemId));

            string basketKey = $"basket:{username}";
            var redisBasket = await _redisService.GetAsync<BasketDTO>(basketKey);

            if (redisBasket == null)
                throw new Exception($"Basket not found for user {username}");

            var basketItemDTO = redisBasket.Items.FirstOrDefault(item => item.BasketItemId == basketItemId);

            if (basketItemDTO == null)
                throw new Exception($"Basket item with ID {basketItemId} not found");

            basketItemDTO.Status = !basketItemDTO.Status;

            await _redisService.SetAsync<BasketDTO>(basketKey, redisBasket, TimeSpan.FromMinutes(1));
        }

        public async Task RemoveRangeItems(string username, int userId, Guid basketId)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username cannot be empty", nameof(username));

            string basketKey = $"basket:{username}";

            // 1. Delete key on redis
            await _redisService.RemoveAsync(basketKey);

            // 2. Delete items on database
            await _unitOfWork.Basket.DeleteBasket(userId, basketId);
            await _unitOfWork.Basket.DeleteBasketItem(basketId);
        }

        #endregion
    }
}
