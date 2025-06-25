using Store_API.DTOs.Baskets;

namespace Store_API.Contracts
{
    public record StockHoldCreated(string PaymentIntentId, int UserId, List<BasketItemDTO> Items);
    public record StockHoldExpiredCreated(string PaymentIntentId);
    public record StockHoldConfirmedCreated(string PaymentIntentId);

    public class StockHoldItem
    {
        public Guid ProductDetailId { get; set; }
        public int Quantity { get; set; }
    }
}
