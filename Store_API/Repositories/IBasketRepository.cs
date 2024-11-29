using Store_API.DTOs.Baskets;

namespace Store_API.Repositories
{
    public interface IBasketRepository
    {
        public Task<BasketDTO> GetBasket(string currentUserLogin);
        public Task AddItem(string userId, int productId, int quantity);
        public Task RemoveItem(string userId, int productId, int quantity);
        public Task UpdateBasketPayment(string paymentIntentId, string clientSecret, string username);
        public Task ToggleStatusItems(string username, int itemId);
    }
}
