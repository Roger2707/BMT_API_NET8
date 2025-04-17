using System.ComponentModel.DataAnnotations.Schema;

namespace Store_API.Models.OrderAggregate
{
    public class OrderItem
    {
        public int Id { get; set; }

        public int OrderId { get; set; }
        [ForeignKey("OrderId")]
        public Order Order { get; set; }

        public Guid ProductDetailId { get; set; }
        [ForeignKey("ProductDetailId")]
        public ProductDetail ProductDetail { get; set; }

        public int Quantity { get; set; }
        public double SubTotal { get; set; }
    }
}
