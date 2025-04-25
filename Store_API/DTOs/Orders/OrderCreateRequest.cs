using Store_API.DTOs.Baskets;
using Store_API.Models.OrderAggregate;

namespace Store_API.DTOs.Orders
{
    public class OrderCreateRequest
    {
        public Guid OrderId { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public ShippingAddress ShippingAdress { get; set; }
        public BasketDTO BasketDTO { get; set; }
        public double Amount { get; set; }
        public string ClientSecret { get; set; }
    }
}
