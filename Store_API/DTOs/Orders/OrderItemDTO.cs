using System.ComponentModel.DataAnnotations.Schema;

namespace Store_API.DTOs.Orders
{
    public class OrderItemDTO
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImageUrl { get; set; }
        public int Quantity { get; set; }
        public double SubTotal { get; set; } 
    }
}
