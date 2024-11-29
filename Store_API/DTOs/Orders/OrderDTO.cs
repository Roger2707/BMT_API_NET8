using Store_API.Models.Order;

namespace Store_API.DTOs.Orders
{
    public class OrderDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string FullName { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
        public double DeliveryFee { get; set; }
        public ShippingAddress ShippingAddress { get; set; }
        public List<OrderItemDTO> Items { get; set; }
        public double TotalPrice { get; set; }
    }
}
