using System.ComponentModel.DataAnnotations;

namespace Store_API.DTOs.Accounts
{
    public class UserProfileDTO
    {
        public string FullName { get; set; }
        public DateTime Dob { get; set; }
        [Phone]
        public string PhoneNumber { get; set; }
        public IFormFile? ImageUrl { get; set; }
    }
}
