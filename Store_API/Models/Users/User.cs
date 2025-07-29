using Store_API.Models.OrderAggregate;

namespace Store_API.Models.Users
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime Dob { get; set; } = DateTime.Now;
        public string ImageUrl { get; set; } = string.Empty;
        public string PublicId { get; set; } = string.Empty;
        public string Provider { get; set; } = "System";

        public List<UserRole> UserRoles { get; set; }
        public Basket Basket { get; set; }
        public List<UserAddress> UserAddresses { get; set; }
        public List<Order> Orders { get; set; }
    }
}
