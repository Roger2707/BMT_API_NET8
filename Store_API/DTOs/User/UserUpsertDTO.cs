using Store_API.DTOs.User;
using System.ComponentModel.DataAnnotations;

namespace Store_API.DTOs.Accounts
{
    public class UserUpsertDTO
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public DateTime Dob { get; set; }
        [Phone]
        public string PhoneNumber { get; set; }
        public string? ImageUrl { get; set; }
        public string? PublicId { get; set; }
        IEnumerable<UserAddressDTO>? UserAddresses { get; set; }
    }
}
