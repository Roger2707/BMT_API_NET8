using System.ComponentModel.DataAnnotations.Schema;

namespace Store_API.Models.OrderAggregate
{
    public class OrderItem
    {
        public int Id { get; set; }

        public Guid OrderId { get; set; }
        [ForeignKey("OrderId")]
        public Order Order { get; set; }

        public ProductOrderItem ProductOrderItem { get; set; }

        public int Quantity { get; set; }
        public double SubTotal { get; set; }
    }
}
