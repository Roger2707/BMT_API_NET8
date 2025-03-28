using Store_API.DTOs;
using Store_API.DTOs.Accounts;
using Store_API.DTOs.User;
using Store_API.Models.Users;

namespace Store_API.IService
{
    public interface IUserService
    {
        #region Retrieve
        Task<Result<List<UserDTO>>> GetAll();
        Task<Result<UserDTO>> GetCurrentUser(string userName);
        #endregion

        #region Email Send
        Task SendWelcomeEmailAsync(string email, string username, string password);
        Task SendLinkResetPwToMail(ForgetPasswordDTO model, User user);
        #endregion

        #region Authentication
        Task<Result<User>> CreateUserAsync(SignUpRequest request);
        Task<Result<User>> AssignRoleAsync(User user, string role);
        Task<Result<LoginResponse>> SignInAsync(LoginRequest request);
        Task<Result<LoginResponse>> ExternalLoginRedirect();
        Task<Result<LoginResponse>> ExternalLoginPopUp(GoogleAuthRequest request);
        #endregion

        #region Password Handles
        Task<Result<string>> ChangePassword(string userName, ChangePasswordDTO changePasswordDTO);
        Task<Result<string>> ForgetPassword(ForgetPasswordDTO forgetPasswordDTO);
        Task<Result<string>> ResetPassword(ResetPasswordDTO resetPasswordDTO);
        #endregion

        #region Methods

        Task<Result<dynamic>> UpdateUserProfile(string userName, UserProfileDTO userProfileDTO);

        #endregion

    }
}
