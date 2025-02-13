using Store_API.DTOs.Baskets;

namespace Store_API.Repositories
{
    public interface IBasketRepository
    {
        public Task<BasketDTO> GetBasket(string currentUserLogin);
        public Task HandleBasketMode(int userId, int productId, bool mode);
        public Task<BasketDTO> UpdateBasketPayment(string paymentIntentId, string clientSecret, string username);
        public Task ToggleStatusItems(string username, int itemId);
    }
}
