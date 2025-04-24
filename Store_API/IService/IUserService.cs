using Store_API.DTOs.Accounts;
using Store_API.DTOs.User;
using Store_API.Models.Users;

namespace Store_API.IService
{
    public interface IUserService
    {
        #region Retrieve
        Task<List<UserDTO>> GetAll();
        Task<UserDTO> GetUser(string userName);
        Task<UserDTO> GetUser(int userId);
        #endregion

        #region Email Send
        Task SendWelcomeEmailAsync(string email, string username, string password);
        Task SendLinkResetPwToMail(ForgetPasswordDTO model, User user);
        #endregion

        #region Authentication
        Task<User> CreateUserAsync(SignUpRequest request);
        Task<UserDTO> SignInAsync(LoginRequest request);
        Task<UserDTO> LoginOAuthRedirect();
        Task<UserDTO> LoginOAuth(GoogleAuthRequest request);
        #endregion

        #region Password Handles
        Task ChangePassword(string userName, ChangePasswordDTO changePasswordDTO);
        Task ForgetPassword(ForgetPasswordDTO forgetPasswordDTO);
        Task ResetPassword(ResetPasswordDTO resetPasswordDTO);
        #endregion

        #region Methods

        Task<string> UpdateUser(UserDTO userUpserDTO);

        #endregion

    }
}
