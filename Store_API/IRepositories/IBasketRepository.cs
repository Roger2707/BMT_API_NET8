using Store_API.DTOs;
using Store_API.DTOs.Baskets;
using Store_API.Models;

namespace Store_API.Repositories
{
    public interface IBasketRepository
    {
        public Task<BasketDTO> GetBasket(string currentUserLogin);
        Task<int> GetBasketIdByUsername(string username);
        public Task HandleBasketMode(int userId, int productId, bool mode);
        public Task<int> UpdateBasketPayment(string paymentIntentId, string clientSecret, string username);
        public Task<Result<int>> ToggleStatusItems(string username, int itemId);
        public Task RemoveRange(List<BasketItemDTO> items);
    }
}
