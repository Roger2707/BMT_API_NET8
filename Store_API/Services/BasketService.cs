using Store_API.Cache_Layer;
using Store_API.DTOs.Baskets;
using Store_API.Enums;
using Store_API.IService;
using Store_API.Repositories;

namespace Store_API.Services
{
    public class BasketService : IBasketService
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IRedisService _redisService;
        private readonly IUnitOfWork _unitOfWork;

        public BasketService(IBasketRepository basketRepository, IRedisService redisService, IUnitOfWork unitOfWork)
        {
            _basketRepository = basketRepository;
            _redisService = redisService;
            _unitOfWork = unitOfWork;
        }

        #region Retrieve & Sync Data

        public async Task<BasketDTO> GetBasketDTORedis(int userId, string username)
        {
            var basketCache = await _basketRepository.GetBasketDTORedis(userId, username);
            return basketCache;
        }

        public async Task<BasketDTO> GetBasketDTODB(string username)
        {
            var basketDb = await _basketRepository.GetBasketDTODB(username);
            return basketDb;
        }

        public async Task SyncBasketDB(string username)
        {
            string basketKey = $"basket:{username}";
            var redisBasket = await _redisService.GetAsync<BasketDTO>(basketKey);
            if (redisBasket == null) return;

            try
            {
                await _unitOfWork.BeginTransactionDapperAsync();

                if (await _basketRepository.CheckBasketExistedDB(redisBasket.UserId, redisBasket.Id))
                {
                    await _basketRepository.DeleteBasketItem(redisBasket.Id);
                    await _basketRepository.DeleteBasket(redisBasket.UserId, redisBasket.Id);
                }
                await _basketRepository.InsertBasket(redisBasket);
                await _basketRepository.InsertBasketItems(redisBasket);

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
            string basketKey = $"basket:{basketUpsertDTO.Username}";
            var redisBasket = await _redisService.GetAsync<BasketDTO>(basketKey);

            if (redisBasket == null)
            {
                var basketFromDB = await GetBasketDTODB(basketUpsertDTO.Username);
                if (basketFromDB == null)
                {
                    redisBasket = new BasketDTO
                    {
                        Id = Guid.NewGuid(),
                        UserId = basketUpsertDTO.UserId,
                        Items = new List<BasketItemDTO>()
                    };
                }
                else
                {
                    redisBasket = basketFromDB;
                }
            }

            //////////
            var basketItemDTO = redisBasket.Items.FirstOrDefault(item => item.ProductDetailId == basketUpsertDTO.ProductDetailId);
            if (basketItemDTO == null)
            {                           
                basketItemDTO = new BasketItemDTO
                {
                    BasketItemId = Guid.NewGuid(),
                    ProductDetailId = basketUpsertDTO.ProductDetailId,
                    Quantity = basketUpsertDTO.Quantity,
                    Status = basketItemDTO.Status,
                };
            }
            else
            {
                if (basketUpsertDTO.Mode == BasketMode.Add)
                {
                    basketItemDTO.Quantity += basketUpsertDTO.Quantity;
                }
                else if (basketUpsertDTO.Mode == BasketMode.Remove)
                {
                    basketItemDTO.Quantity -= basketUpsertDTO.Quantity;

                    // If result quantity < 0
                    if (basketItemDTO.Quantity <= 0)
                        redisBasket.Items.Remove(basketItemDTO);
                }
            }

            await _redisService.SetAsync<BasketDTO>(basketKey, redisBasket, TimeSpan.FromMinutes(30));
        }

        public async Task ToggleBasketItemStatus(string username, Guid basketItemId)
        {
            string basketKey = $"basket:{username}";
            var redisBasket = await _redisService.GetAsync<BasketDTO>(basketKey);

            if (redisBasket == null)
                throw new Exception("Items is not found !");

            var basketItemDTO = redisBasket.Items.FirstOrDefault(item => item.BasketItemId == basketItemId);

            if (basketItemDTO == null)
                throw new Exception("Basket item not found!");

            // Update Status = !Current Status
            basketItemDTO.Status = !basketItemDTO.Status;

            await _redisService.SetAsync<BasketDTO>(basketKey, redisBasket, TimeSpan.FromMinutes(30));
        }

        public async Task RemoveRangeItems(string username)
        {
            string basketKey = $"basket:{username}";
            var redisBasket = await _redisService.GetAsync<BasketDTO>(basketKey);

            if (redisBasket == null)
                throw new Exception("Items is not found !");

            redisBasket.Items.Clear();
            await _redisService.SetAsync<BasketDTO>(basketKey, redisBasket, TimeSpan.FromMinutes(30));
        }

        #endregion
    }
}
