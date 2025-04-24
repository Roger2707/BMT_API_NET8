using Store_API.DTOs.Baskets;

namespace Store_API.DTOs.Orders
{
    public class OrderCreateRequest
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public int UserAddressId { get; set; }
        public BasketDTO BasketDTO { get; set; }
        public double Amount { get; set; }
        public string ClientSecret { get; set; }
    }
}
