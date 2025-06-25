using Store_API.Enums;

namespace Store_API.Models.Inventory
{
    public class StockHold
    {
        public Guid Id { get; set; }
        public int UserId { get; set; }
        public string PaymentIntentId { get; set; }
        public StockHoldStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }

        public List<StockHoldItem> Items { get; set; }
    }

    public class StockHoldItem
    {
        public Guid Id { get; set; }
        public Guid StockHoldId { get; set; }
        public Guid ProductDetailId { get; set; }
        public int Quantity { get; set; }
        public StockHold StockHold { get; set; }
    }
}
