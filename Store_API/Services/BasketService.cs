using Store_API.DTOs.Baskets;
using Store_API.Enums;
using Store_API.IService;
using Store_API.IRepositories;

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

            var basketCache = await _unitOfWork.Basket.GetBasketDTORedis(userId, username);
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

        #region CRUD Operations (Action Implement on Redis)

        public async Task UpsertBasket(BasketUpsertDTO basketUpsertDTO)
        {
            if (basketUpsertDTO == null)
                throw new ArgumentNullException(nameof(basketUpsertDTO));

            if (string.IsNullOrWhiteSpace(basketUpsertDTO.Username))
                throw new ArgumentException("Username cannot be empty", nameof(basketUpsertDTO.Username));

            if (basketUpsertDTO.Quantity <= 0)
                throw new ArgumentException("Quantity must be greater than 0", nameof(basketUpsertDTO.Quantity));

            string basketKey = $"basket:{basketUpsertDTO.Username}";
            var redisBasket = await _redisService.GetAsync<BasketDTO>(basketKey);

            try
            {
                redisBasket = await GetOrCreateBasket(redisBasket, basketUpsertDTO);
                await UpdateBasketItems(redisBasket, basketUpsertDTO);
                await _redisService.SetAsync<BasketDTO>(basketKey, redisBasket, TimeSpan.FromMinutes(1));
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to upsert basket for user {basketUpsertDTO.Username}", ex);
            }
        }

        private async Task<BasketDTO> GetOrCreateBasket(BasketDTO redisBasket, BasketUpsertDTO basketUpsertDTO)
        {
            if (redisBasket != null) return redisBasket;

            var basketFromDB = await _unitOfWork.Basket.GetBasketDTODB(basketUpsertDTO.Username);
            if (basketFromDB != null) return basketFromDB;

            return new BasketDTO
            {
                Id = Guid.NewGuid(),
                UserId = basketUpsertDTO.UserId,
                Items = new List<BasketItemDTO>()
            };
        }

        private async Task UpdateBasketItems(BasketDTO basket, BasketUpsertDTO basketUpsertDTO)
        {
            var existingItem = basket.Items.FirstOrDefault(item => item.ProductDetailId == basketUpsertDTO.ProductDetailId);

            if (existingItem == null)
            {
                var productDetail = await _productService.GetProductSingleDetail(basketUpsertDTO.ProductDetailId);
                
                if (productDetail == null)
                    throw new Exception($"Product detail with ID {basketUpsertDTO.ProductDetailId} not found");

                basket.Items.Add(new BasketItemDTO
                {
                    BasketItemId = Guid.NewGuid(),
                    ProductDetailId = productDetail.ProductDetailId,
                    Quantity = basketUpsertDTO.Quantity,
                    Status = true,
                    ProductName = productDetail.ProductName,
                    ProductFirstImage = productDetail.ProductFirstImage,
                    OriginPrice = productDetail.OriginPrice,
                    DiscountPercent = productDetail.DiscountPercent,
                    DiscountPrice = productDetail.DiscountPrice
                });
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
