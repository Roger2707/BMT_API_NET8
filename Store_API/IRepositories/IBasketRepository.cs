using Store_API.DTOs.Baskets;

namespace Store_API.Repositories
{
    public interface IBasketRepository
    {
        #region Basic CRUD

        Task<bool> CheckBasketExistedDB(int userId, Guid basketId);
        Task DeleteBasket(int userId, Guid basketId);
        Task DeleteBasketItem(Guid basketId);
        Task InsertBasket(BasketDTO basketDTO);
        Task InsertBasketItems(BasketDTO basketDTO);

        #endregion

        #region Retrieve

        Task<BasketDTO> GetBasketDTORedis(int userId, string username);
        Task<BasketDTO> GetBasketDTODB(string username);

        #endregion
    }
}
