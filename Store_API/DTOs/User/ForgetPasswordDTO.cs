using System.ComponentModel.DataAnnotations;

namespace Store_API.DTOs.Accounts
{
    public class ForgetPasswordDTO
    {
        [RegularExpression("\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*", ErrorMessage = "Email Address is invalid")]
        public string Email { get; set; }
    }
}
