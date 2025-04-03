namespace Store_API.DTOs.Baskets
{
    public class BasketDapperRow
    {
        public Guid Id { get; set; }
        public int UserId { get; set; }
        public Guid BasketItemId { get; set; }
        public Guid ProducDetailId { get; set; }
        public string ProductName { get; set; }
        public string ProductFirstImage { get; set; }
        public int Quantity { get; set; }
        public double OriginPrice { get; set; }
        public double DiscountPercent { get; set; }
        public double DiscountPrice { get; set; }
        public bool Status { get; set; }
        public double GrandTotal { get; set; } = 0;
    }
}
