namespace Store_API.DTOs.Orders
{
    public class OrderItemDTO
    {
        public int Id { get; set; }
        public Guid ProductDetailId { get; set; }
        public string ProductName { get; set; }
        public string ProductImageUrl { get; set; }
        public int Quantity { get; set; }
        public double SubTotal { get; set; } 
    }
}
