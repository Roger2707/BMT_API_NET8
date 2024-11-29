namespace Store_API.Models.Order
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; }
        public double DeliveryFee { get; set; }
        public ShippingAddress ShippingAddress { get; set; }
        public List<OrderItem> Items { get; set; }
        public double TotalPrice { get; set; }
        public string PaymentIntentId { get; set; }
    }
}
