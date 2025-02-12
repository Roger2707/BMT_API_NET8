namespace Store_API.DTOs.Orders
{
    public class OrderProcessedMessage
    {
        public string PaymentIntentId { get; set; }
        public string Status { get; set; }
    }
}
