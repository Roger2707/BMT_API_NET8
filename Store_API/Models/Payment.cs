using Store_API.Enums;
using Store_API.Models.OrderAggregate;
using System.ComponentModel.DataAnnotations.Schema;

namespace Store_API.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public string PaymentIntentId { get; set; } // ✅ ID từ Stripe
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
        public double Amount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int OrderId { get; set; }

        [ForeignKey("OrderId")]
        public Order Order { get; set; }
    }
}
