using Store_API.DTOs.User;

namespace Store_API.DTOs.Accounts
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public DateTime Dob { get; set; }
        public string PhoneNumber { get; set; }
        public string? ImageUrl { get; set; }
        public string? PublicId { get; set; }
        public string RoleName { get; set; }
        public string Provider { get; set; }
        public Guid BasketId { get; set; }
        public IEnumerable<UserAddressDTO>? UserAddresses { get; set; }
    }
}
