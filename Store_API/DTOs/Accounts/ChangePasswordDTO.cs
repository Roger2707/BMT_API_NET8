namespace Store_API.DTOs.Accounts
{
    public class ChangePasswordDTO
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmedNewPassword { get; set; }
    }
}
