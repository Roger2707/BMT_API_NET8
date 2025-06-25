using Store_API.DTOs.Baskets;

namespace Store_API.IService
{
    public interface IStockHoldService
    {
        Task CreateStockHoldAsync(string paymentIntentId, int userId, List<BasketItemDTO> basketItems);
        Task ConfirmStockHoldAsync(string paymentIntentId, int userId);
    }
}
