using Store_API.Enums;

namespace Store_API.DTOs.Orders
{
    public class OrderResponseDTO
    {
        public Guid Id { get; set; }
        public double GrandTotal { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public string ClientSecret { get; set; }
    }
}
