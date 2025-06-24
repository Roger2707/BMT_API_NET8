namespace Store_API.Contracts
{
    public record StockHoldCreated(string paymentIntentId, int userId, List<StockHoldItem> items);
    public record StockReleasedCreated(string paymentIntentId);
    public record StockConfirmedCreated(string paymentIntentId);

    public class StockHoldItem
    {
        public Guid ProductDetailId { get; set; }
        public int Quantity { get; set; }
    }
}
