namespace Store_API.Models
{
    public class PaymentIntentRecord
    {
        public int Id { get; set; }
        public string StripePaymentIntentId { get; set; }
        public string ClientSecret { get; set; }
        public string UserId { get; set; }
        public string CartHash { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
