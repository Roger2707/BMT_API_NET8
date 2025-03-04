using Store_API.Models.OrderAggregate;
using System.ComponentModel.DataAnnotations.Schema;

namespace Store_API.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public string PaymentIntentId { get; set; } // ✅ ID từ Stripe
        public OrderStatus Status { get; set; } = OrderStatus.Pending; // Pending, Paid, Failed, Refunded
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int OrderId { get; set; }

        [ForeignKey("OrderId")]
        public Order Order { get; set; }
    }
}
