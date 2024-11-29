using System.ComponentModel.DataAnnotations;

namespace Store_API.DTOs.Accounts
{
    public class SignUpRequest
    {
        public string FullName { get; set; }
        [RegularExpression("\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*", ErrorMessage = "Email Address is invalid")]
        public string Email { get; set; }
        [Required(ErrorMessage = "UserName is required")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Phone Number is required")]
        public string PhoneNumber { get; set; }
    }
}
