using Store_API.DTOs.Baskets;

namespace Store_API.Contracts
{
    public record StockHoldCreated(string PaymentIntentId, int UserId, List<BasketItemDTO> Items);
    public record StockHoldExpiredCreated(string PaymentIntentId);
}
