using Microsoft.AspNetCore.Identity;

namespace Store_API.Models
{
    public class User : IdentityUser<int>
    {
        public string FullName { get; set; }
        public DateTime Dob { get; set; }
        public string? ImageUrl { get; set; }
        public string? PublicId { get; set; }
        public Basket Basket { get; set; }
    }
}
