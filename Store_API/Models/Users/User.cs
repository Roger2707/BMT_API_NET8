using Microsoft.AspNetCore.Identity;
using Store_API.Models.OrderAggregate;

namespace Store_API.Models.Users
{
    public class User : IdentityUser<int>
    {
        public string FullName { get; set; }
        public DateTime Dob { get; set; }
        public string ImageUrl { get; set; }
        public string PublicId { get; set; }
        public string Provider { get; set; } = "System";
        public Basket Basket { get; set; }
        public List<UserAddress> UserAddresses { get; set; }
        public List<Order> Orders { get; set; }
    }
}
