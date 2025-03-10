using Store_API.DTOs;
using Store_API.DTOs.Baskets;

namespace Store_API.IService
{
    public interface IBasketService
    {
        Task<BasketDTO> GetBasket(string userName);
        Task<Result<BasketDTO>> HandleBasketMode(int userId, int productId, bool mode, string currentUserName);
        Task<Result<int>> ToggleStatusItems(string username, int itemId);
        Task<Result<int>> RemoveRange(string username, List<BasketItemDTO> items);
    }
}
