using Store_API.DTOs.Baskets;

namespace Store_API.Repositories.IRepositories
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

        Task<BasketDTO> GetBasket(string username);
        Task<BasketDTO> GetBasketFromDB(string username);

        #endregion
    }
}
