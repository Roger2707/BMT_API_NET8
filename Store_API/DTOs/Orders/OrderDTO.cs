using Store_API.Models.OrderAggregate;

namespace Store_API.DTOs.Orders
{
    public class OrderDTO
    {
        public Guid Id { get; set; }
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
        public double DeliveryFee { get; set; }
        public ShippingAdress ShippingAdress { get; set; }
        public List<OrderItemDTO> Items { get; set; }
        public double GrandTotal { get; set; }
        public string ClientSecret { get; set; }
    }
}
