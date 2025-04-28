using Store_API.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Store_API.Models.OrderAggregate
{
    public class ShippingOrder
    {
        public int Id { get; set; }
        public Guid OrderId { get; set; }
        [ForeignKey("OrderId")]
        public Order Order { get; set; }
        public string GHNOrderCode { get; set; }
        public ShippingStatus ShippingStatus { get; set; } = ShippingStatus.Pending;

        // Shipping Address
        public string ToName { get; set; }
        public string ToPhone { get; set; }
        public string ToAddress { get; set; }
        public string ToWard { get; set; }
        public string ToDistrict { get; set; }
        public string ToProvince { get; set; }

        // Condition Required
        public int Weight { get; set; }
        public int Length { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public double CODAmount { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
