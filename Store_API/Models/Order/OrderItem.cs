using System.ComponentModel.DataAnnotations.Schema;

namespace Store_API.Models.Order
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        [ForeignKey("OrderId")]
        public Order Order { get; set; }    
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public double ItemPrice { get; set; }
    }
}
