using Store_API.Enums;

namespace Store_API.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int UserAddressId { get; set; }
        public Guid OrderId { get; set; }
        public string PaymentIntentId { get; set; }
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
        public double Amount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
