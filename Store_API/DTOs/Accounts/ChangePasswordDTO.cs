using System.ComponentModel.DataAnnotations;

namespace Store_API.DTOs.Accounts
{
    public class ChangePasswordDTO
    {
        [Required]
        public string CurrentPassword { get; set; }
        [Required]
        public string NewPassword { get; set; }
        [Required]
        public string ConfirmedNewPassword { get; set; }
    }
}
