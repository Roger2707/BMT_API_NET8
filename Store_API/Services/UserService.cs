using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using Store_API.Data;
using Store_API.DTOs;
using Store_API.DTOs.Accounts;
using Store_API.Helpers;
using Store_API.IService;
using Store_API.Models;
using Store_API.Repositories;
using System.Security.Claims;
using Store_API.DTOs.User;
using Google.Apis.Auth;

namespace Store_API.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        private readonly IUnitOfWork _unitOfWork;
        private readonly IDapperService _dapperService;

        private readonly IImageRepository _imageService;
        private readonly EmailSenderService _emailSenderService;
        private readonly ITokenRepository _tokenService;
        private readonly IConfiguration _configuration;

        public UserService(UserManager<User> userManager, SignInManager<User> signInManager, EmailSenderService emailSenderService
            , IUnitOfWork unitOfWork, ITokenRepository tokenService, IImageRepository imageService, IDapperService dapperService, IConfiguration configuration) 
        {
            _userManager = userManager;
            _signInManager = signInManager;

            _unitOfWork = unitOfWork;
            _dapperService = dapperService;

            _emailSenderService = emailSenderService;
            _tokenService = tokenService;
            _imageService = imageService;
            _configuration = configuration;
        }

        #region Retrieve

        public async Task<Result<List<UserDTO>>> GetAll()
        {
            string query = @" 
                            SELECT 

                                u.Id
                                , u.UserName
                                , u.FullName
                                , u.Email
                                , u.PhoneNumber
                                , u.Dob
                                , u.ImageUrl
                                , r.Name as RoleName
	                            , basket.Id as BasketId

                            FROM AspNetUserRoles user_role

                            LEFT JOIN AspNetUsers u ON u.Id = user_role.UserId
                            LEFT JOIN AspNetRoles r ON r.Id = user_role.RoleId
                            LEFT JOIN Baskets basket ON basket.UserId = user_role.UserId

                            WHERE r.Id != 1 ";


            List<dynamic> result = await _dapperService.QueryAsync(query, null);

            if (result.Count == 0) return null;
            List<UserDTO> users = new List<UserDTO>();

            foreach (var user in result)
            {
                var userDTO = new UserDTO()
                {                  
                    UserName = user.UserName,
                    FullName = user.FullName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Dob = user.Dob,
                    ImageUrl = user.ImageUrl,
                    RoleName = user.RoleName,
                    BasketId = user.BasketId ?? 0
                };

                users.Add(userDTO);
            }

            return Result<List<UserDTO>>.Success(users);
        }

        public async Task<Result<UserDTO>> GetCurrentUser(string userName)
        {
            string query = @" 
                            SELECT 
                                u.Id
                                , FullName
                                , UserName
                                , Email
                                , Dob
                                , PhoneNumber
                                , ImageUrl
                                , b.Id as BasketId
                                FROM AspNetUsers u
								LEFT JOIN Baskets b ON b.UserId = u.Id
                                WHERE UserName = @UserName
                                ";

            var user = await _dapperService.QueryFirstOrDefaultAsync(query, new { UserName = userName });
            if (user == null) return null;

            var respone = new UserDTO
            {
                FullName = user.FullName,
                UserName = user.UserName,
                Email = user.Email,
                Dob = user.Dob,
                PhoneNumber = user.PhoneNumber,
                ImageUrl = user.ImageUrl,
                Token = await _tokenService.GenerateToken(new User { Id = user.Id, Email = user.Email, UserName = user.UserName }),
                BasketId  = CF.GetInt(user.BasketId)
            };
            return Result<UserDTO>.Success(respone);
        }

        #endregion 

        #region Sign Up

        public async Task<Result<User>> CreateUserAsync(SignUpRequest request)
        {
            if (await _userManager.FindByEmailAsync(request.Email) != null)
                return Result<User>.Failure("Email already exists.");

            if (await _userManager.FindByNameAsync(request.Username) != null)
                return Result<User>.Failure("Username already exists.");

            string password = request.Username[0].ToString().ToUpper() + request.Username.Substring(1).ToLower() + "@123";

            var user = new User
            {
                FullName = request.FullName,
                Email = request.Email,
                UserName = request.Username,
                PhoneNumber = request.PhoneNumber,
                PasswordHash = password,
            };

            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
                return Result<User>.Failure(result.Errors.Select(e => e.Description).ToList());

            return Result<User>.Success(user);
        }

        public async Task<Result<User>> AssignRoleAsync(User user, string role)
        {
            var result = await _userManager.AddToRoleAsync(user, role);
            if (!result.Succeeded)
                return Result<User>.Failure(result.Errors.Select(e => e.Description).ToList());

            return Result<User>.Success(user);
        }

        public async Task SendWelcomeEmailAsync(string email, string username, string password)
        {
            string content = $@"    Hello, welcome to our badminton store
                                    Your Initial info:

                                    Username: {username}
                                    Password: {password}

                                    You can change your password after sign in
                                    Wish you 'll have a nice experience. Thank you!
                                                                            [ROGER]                                       
                                ";

            await _emailSenderService.SendEmailAsync(email, "[NEW] 🔥 Your Account:", content);
        }

        #endregion

        #region Log in
        public async Task<Result<LoginResponse>> SignInAsync(LoginRequest request)
        {
            var exitedUser = await _userManager.FindByNameAsync(request.Username);
            if (exitedUser == null || !await _userManager.CheckPasswordAsync(exitedUser, request.Password))
                return Result<LoginResponse>.Failure("Username or password is incorrect.");

            await _signInManager.SignInAsync(exitedUser, isPersistent: false);

            var userResponse = new LoginResponse
            {
                FullName = exitedUser.FullName,
                Email = exitedUser.Email,
                Token = await _tokenService.GenerateToken(exitedUser)
            };
            return Result<LoginResponse>.Success(userResponse);
        }

        #endregion

        #region OAUth
        public async Task<Result<LoginResponse>> ExternalLoginRedirect()
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
                return Result<LoginResponse>.Failure("Error loading external login information.");

            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
            if (result.Succeeded)
            {
                var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
                var userResponse = new LoginResponse
                {
                    FullName = user.FullName,
                    Email = user.Email,
                    Token = await _tokenService.GenerateToken(user)
                };
                return Result<LoginResponse>.Success(userResponse);
            }
            else
            {
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                var fullName = info.Principal.FindFirstValue(ClaimTypes.Name);

                var user = new User { UserName = email, Email = email, FullName = fullName };

                var createResult = await _userManager.CreateAsync(user);

                if (createResult.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Customer");
                    await _userManager.AddLoginAsync(user, info);
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    var userResponse = new LoginResponse
                    {
                        FullName = fullName,
                        Email = email,
                        Token = await _tokenService.GenerateToken(user)
                    };
                    return Result<LoginResponse>.Success(userResponse);
                }
            }
            return Result<LoginResponse>.Failure("Something went wrong.");
        }

        public async Task<Result<LoginResponse>> ExternalLoginPopUp(GoogleAuthRequest request)
        {
            if (string.IsNullOrEmpty(request.AuthCode))
                return Result<LoginResponse>.Failure("Authorization Code is required");

            var tokenRequestUri = "https://oauth2.googleapis.com/token";
            var requestBody = new Dictionary<string, string>
            {
                { "code", request.AuthCode },
                { "client_id", _configuration["OAuth:ClientID"] },
                { "client_secret", _configuration["OAuth:ClientSecret"] },
                { "redirect_uri", "postmessage" },
                { "grant_type", "authorization_code" }
            };

            using var httpClient = new HttpClient();
            var response = await httpClient.PostAsync(tokenRequestUri, new FormUrlEncodedContent(requestBody));
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return Result<LoginResponse>.Failure("Error: Authorization Code");


            var tokenResponse = JsonConvert.DeserializeObject<GoogleTokenResponse>(responseString);
            if (tokenResponse == null || string.IsNullOrEmpty(tokenResponse.Id_token))
                return Result<LoginResponse>.Failure("Could not retrieve id_token");

            var settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new[] { _configuration["OAuth:ClientID"] }
            };
            var payload = GoogleJsonWebSignature.ValidateAsync(tokenResponse.Id_token, settings).Result;
            var user = await _userManager.FindByEmailAsync(payload.Email);

            if (user == null)
            {
                user = new User { UserName = payload.Email, Email = payload.Email, FullName = payload.Name };
                var createResult = await _userManager.CreateAsync(user);

                if (!createResult.Succeeded)
                    return Result<LoginResponse>.Failure("Failed to create user");

                await _userManager.AddToRoleAsync(user, "Customer");
            }
            var token = await _tokenService.GenerateToken(user);

            // Log in into system -> set User.identity.IsAuthenticated
            await _signInManager.SignInAsync(user, isPersistent: false);
            var userResponse = new LoginResponse
            {
                FullName = payload.Name,
                Email = payload.Email,
                Token = token
            };
            return Result<LoginResponse>.Success(userResponse);
        }

        #endregion

        #region Password Methods
        public async Task<Result<string>> ChangePassword(string userName, ChangePasswordDTO changePasswordDTO)
        {
            var user = await _userManager.FindByNameAsync(userName);
            bool checkResult = await _userManager.CheckPasswordAsync(user, changePasswordDTO.CurrentPassword);
            if (!checkResult)
                return Result<string>.Failure("Current Password is incorrect.");

            if (string.Compare(changePasswordDTO.NewPassword, changePasswordDTO.ConfirmedNewPassword) != 0)
                return Result<string>.Failure("New Password and ConfirmedPassword must be the same !");

            var result = await _userManager.ChangePasswordAsync(user, changePasswordDTO.CurrentPassword, changePasswordDTO.NewPassword);

            if (!result.Succeeded)
            {
                var errors = new List<string>();

                foreach (var error in result.Errors)
                {
                    errors.Add(error.Description);
                }
                return Result<string>.Failure(string.Join(", ", errors));
            }
            return Result<string>.Success("Password is Changed Successfully !");
        }

        public async Task<Result<string>> ForgetPassword(ForgetPasswordDTO forgetPasswordDTO)
        {
            var user = await _userManager.FindByEmailAsync(forgetPasswordDTO.Email);
            if (user == null) return Result<string>.Failure("Email does not exist.");

            // Generate Token (Encode in query string)
            var token = CF.Base64ForUrlEncode(await _userManager.GeneratePasswordResetTokenAsync(user));

            string linkResetPassword = $"http://localhost:3000/get-reset-password?email={forgetPasswordDTO.Email}&token={token}";

            string htmlContent = $"This is your link to reset password :" + Environment.NewLine
               + $"Link :" + linkResetPassword;

            // Send Message to email
            await _emailSenderService.SendEmailAsync(forgetPasswordDTO.Email, "Reset Password ROGER BMT APP (NET 8)", htmlContent);

            return Result<string>.Success($"Link reset password is sent to your email: {forgetPasswordDTO.Email}.");
        }

        public async Task SendLinkResetPwToMail(ForgetPasswordDTO model, User user)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            token = token.Replace(" ", "+");
            token = CF.Base64ForUrlEncode(token);
            string linkResetPassword = $"http://localhost:3000/get-reset-password?email={model.Email}&token={token}";
            string htmlContent = $"This is your link to reset password :" + Environment.NewLine
                + $"Link :" + linkResetPassword;

            // Send Message to email
            await _emailSenderService.SendEmailAsync(model.Email, "Reset Password (ROGER APP)", htmlContent);
        }

        public async Task<Result<string>> ResetPassword(ResetPasswordDTO resetPasswordDTO)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordDTO.Email);

            if (user == null)
                return Result<string>.Failure("Email does not exist.");

            // Compare Password
            if (string.Compare(resetPasswordDTO.NewPassword, resetPasswordDTO.ConfirmNewPassword) != 0)
                return Result<string>.Failure("New Password and ConfirmedPassword must be the same !");

            // Handle Result
            var result = await _userManager.ResetPasswordAsync(user, CF.Base64ForUrlDecode(resetPasswordDTO.Token), resetPasswordDTO.NewPassword);
            if (!result.Succeeded)
            {
                var errors = new List<string>();
                foreach (var error in result.Errors)
                    errors.Add(error.Description);

                return Result<string>.Failure(string.Join(", ", errors));
            }
            return Result<string>.Success("Password reset successfully.");
        }

        #endregion

        #region Methods

        public async Task<Result<dynamic>> UpdateUserProfile(string userName, UserProfileDTO userProfileDTO)
        {
            var user = await _userManager.FindByNameAsync(userName);

            // Update
            if (userProfileDTO.FullName != user.FullName && userProfileDTO.FullName != "")
                user.FullName = userProfileDTO.FullName;
            if (userProfileDTO.PhoneNumber != user.PhoneNumber && userProfileDTO.PhoneNumber != "")
                user.PhoneNumber = userProfileDTO.PhoneNumber;
            if (userProfileDTO.Dob != user.Dob && userProfileDTO.Dob.ToString() != "")
                user.Dob = userProfileDTO.Dob;

            if (userProfileDTO.ImageUrl != null)
            {
                var imageResult = await _imageService.AddImageAsync(userProfileDTO.ImageUrl, "accounts");

                if (imageResult.Error != null) throw new Exception(imageResult.Error.Message);

                user.ImageUrl = imageResult.SecureUrl.ToString();
                user.PublicId = imageResult.PublicId;
            }

            int result = await _unitOfWork.SaveChangesAsync();

            if (result > 0)
            {
                var updatedUser = await _userManager.FindByNameAsync(userName);

                return Result<dynamic>.Success(new
                {
                    FullName = updatedUser.FullName,
                    Dob = updatedUser.Dob,
                    Email = updatedUser.Email,
                    ImageUrl = updatedUser.ImageUrl,
                    PhoneNumber = updatedUser.PhoneNumber,
                    Token = await _tokenService.GenerateToken(updatedUser),

                    Title = "Updated Successfully !"
                });
            }

            return Result<dynamic>.Failure("Update Failed !");
        }

        #endregion
    }
}
