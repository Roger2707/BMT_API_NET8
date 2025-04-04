using Store_API.DTOs.Baskets;

namespace Store_API.IService
{
    public interface IBasketService
    {
        #region Retrieve and Sync

        Task<BasketDTO> GetBasketDTORedis(int userId, string username);
        Task<BasketDTO> GetBasketDTODB(string username);
        Task SyncBasketDB(string username);
        Task CleanupExpiredBaskets();

        #endregion

        #region CRUD Operations

        Task UpsertBasket(BasketUpsertDTO basketUpsertDTO);
        Task ToggleBasketItemStatus(string username, Guid basketItemId);
        Task RemoveRangeItems(string username);

        #endregion
    }
}
