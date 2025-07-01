using Store_API.Enums;

namespace Store_API.Models
{
    public class Payment
    {
        public Guid Id { get; set; }
        public int UserId { get; set; }
        public Guid OrderId { get; set; }
        public string PaymentIntentId { get; set; }
        public string ClientSecret { get; set; }
        public string BasketHash { get; set; }
        public double Amount { get; set; }
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
