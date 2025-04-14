using Store_API.DTOs.Baskets;

namespace Store_API.IService
{
    public interface IBasketService
    {
        #region Retrieve Data

        Task<BasketDTO> GetBasketDTORedis(int userId, string username);
        Task<BasketDTO> GetBasketDTODB(string username);

        #endregion

        #region Sync database Implementations

        Task<IEnumerable<string>> GetBasketKeysAsync();
        Task<TimeSpan?> GetBasketTTLAsync(string key);
        Task SyncBasketDB(string username);

        #endregion

        #region CRUD Operations

        Task UpsertBasket(BasketUpsertDTO basketUpsertDTO);
        Task ToggleBasketItemStatus(string username, Guid basketItemId);
        Task RemoveRangeItems(string username);

        #endregion
    }
}
