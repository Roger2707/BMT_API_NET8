using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using Store_API.DTOs.Accounts;
using Store_API.Helpers;
using Store_API.IService;
using Store_API.Repositories;
using System.Security.Claims;
using Store_API.DTOs.User;
using Google.Apis.Auth;
using Store_API.Models.Users;
using Store_API.Enums;
using Store_API.Models.OrderAggregate;

namespace Store_API.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        private readonly IUnitOfWork _unitOfWork;
        private readonly IDapperService _dapperService;

        private readonly EmailSenderService _emailSenderService;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _configuration;

        public UserService(UserManager<User> userManager, SignInManager<User> signInManager, EmailSenderService emailSenderService
            , IUnitOfWork unitOfWork, ITokenService tokenService, IDapperService dapperService, IConfiguration configuration) 
        {
            _userManager = userManager;
            _signInManager = signInManager;

            _unitOfWork = unitOfWork;
            _dapperService = dapperService;

            _emailSenderService = emailSenderService;
            _tokenService = tokenService;
            _configuration = configuration;
        }

        #region Retrieve

        public async Task<List<UserDTO>> GetAll()
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

                            WHERE r.Id != 1 
";

            List<UserDTO> result = await _dapperService.QueryAsync<UserDTO>(query, null);

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
                    BasketId = user.BasketId,
                };

                users.Add(userDTO);
            }
            return users;
        }

        public async Task<UserDTO> GetUser(string username)
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
                                    , u.PublicId
                                    , ISNULL(b.Id, NULL) as BasketId
                                    , ua.Id as AddressId
                                    , ua.City
                                    , ua.District
                                    , ua.Ward
                                    , ua.StreetAddress
                                    , ua.PostalCode
                                    , ua.Country
                                FROM AspNetUsers u
                                LEFT JOIN Baskets b ON b.UserId = u.Id
                                LEFT JOIN UserAddresses ua ON ua.UserId = u.Id
                                WHERE UserName = @UserName
                                ";

            var user = await _dapperService.QueryAsync<UserDapperRow>(query, new { UserName = username });
            if (user == null) return null;

            var token = await _tokenService.GenerateToken(new User { Id = user[0].Id, Email = user[0].Email, UserName = user[0].UserName });
            var userDTO = user
                .GroupBy(g => new { g.Id, g.UserName, g.FullName, g.Email, g.ImageUrl, g.Dob, g.PublicId, g.PhoneNumber, g.BasketId})
                .Select(u => new UserDTO
                {
                    Id = u.Key.Id,
                    UserName = u.Key.UserName,
                    FullName = u.Key.FullName,
                    Email = u.Key.Email,
                    ImageUrl = u.Key.ImageUrl,
                    Dob = u.Key.Dob,
                    PublicId = u.Key.PublicId,
                    PhoneNumber = u.Key.PhoneNumber,
                    BasketId = u.Key.BasketId,
                    Token = token,
                    UserAddresses = u.Select(a => new UserAddressDTO
                    {
                        Id = a.AddressId,
                        City = a.City,
                        District = a.District,
                        Ward = a.Ward,
                        StreetAddress = a.StreetAddress,
                        PostalCode = a.PostalCode,
                        Country = a.Country
                    }).ToList()
                })
                .FirstOrDefault();

            return userDTO;
        }

        public async Task<UserDTO> GetUser(int userId)
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
                                    , u.PublicId
                                    , ISNULL(b.Id, NULL) as BasketId
                                    , ua.Id as AddressId
                                    , ua.City
                                    , ua.District
                                    , ua.Ward
                                    , ua.StreetAddress
                                    , ua.PostalCode
                                    , ua.Country
                                FROM AspNetUsers u
                                LEFT JOIN Baskets b ON b.UserId = u.Id
                                LEFT JOIN UserAddresses ua ON ua.UserId = u.Id
                                WHERE u.Id = @UserId
                                ";

            var user = await _dapperService.QueryAsync<UserDapperRow>(query, new { UserId = userId });
            if (user == null) return null;

            var token = await _tokenService.GenerateToken(new User { Id = user[0].Id, Email = user[0].Email, UserName = user[0].UserName });
            var userDTO = user
                .GroupBy(g => new { g.Id, g.UserName, g.FullName, g.Email, g.ImageUrl, g.Dob, g.PublicId, g.PhoneNumber, g.BasketId })
                .Select(u => new UserDTO
                {
                    Id = u.Key.Id,
                    UserName = u.Key.UserName,
                    FullName = u.Key.FullName,
                    Email = u.Key.Email,
                    ImageUrl = u.Key.ImageUrl,
                    Dob = u.Key.Dob,
                    PublicId = u.Key.PublicId,
                    PhoneNumber = u.Key.PhoneNumber,
                    BasketId = u.Key.BasketId,
                    Token = token,
                    UserAddresses = u.Select(a => new UserAddressDTO
                    {
                        Id = a.AddressId,
                        City = a.City,
                        District = a.District,
                        Ward = a.Ward,
                        StreetAddress = a.StreetAddress,
                        PostalCode = a.PostalCode,
                        Country = a.Country
                    }).ToList()
                })
                .FirstOrDefault();

            return userDTO;
        }

        #endregion 

        #region Sign Up

        public async Task<User> CreateUserAsync(SignUpRequest request)
        {
            if (await _userManager.FindByEmailAsync(request.Email) != null)
                throw new Exception("Email already exists.");

            if (await _userManager.FindByNameAsync(request.Username) != null)
                throw new Exception("Username already exists.");

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
            await _userManager.AddToRoleAsync(user, "Customer");

            if (!result.Succeeded)
            {
                var errors = new List<string>();

                foreach (var error in result.Errors)
                {
                    errors.Add(error.Description);
                }
                throw new Exception(string.Join(", ", errors));
            }

            return user;
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
        public async Task<UserDTO> SignInAsync(LoginRequest request)
        {
            var exitedUser = await _userManager.FindByNameAsync(request.Username);
            if (exitedUser == null || !await _userManager.CheckPasswordAsync(exitedUser, request.Password))
                throw new Exception("Username or password is incorrect.");

            var userResponse = await GetUser(exitedUser.UserName);
            return userResponse;
        }

        #endregion

        #region OAUth 2.0 (Google)
        public async Task<UserDTO> LoginOAuthRedirect()
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
                throw new Exception("External login information is null.");

            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
            if (result.Succeeded)
            {
                var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
                var userResponse = await GetUser(user.UserName);
                return userResponse;
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

                    var userResponse = await GetUser(user.UserName);
                    return userResponse;
                }
            }
            return null;
        }

        public async Task<UserDTO> LoginOAuth(GoogleAuthRequest request)
        {
            if (string.IsNullOrEmpty(request.AuthCode))
                throw new Exception("Authorization code is null or empty.");

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
                throw new Exception($"Failed to login {responseString}");

            var tokenResponse = JsonConvert.DeserializeObject<GoogleTokenResponse>(responseString);
            if (tokenResponse == null || string.IsNullOrEmpty(tokenResponse.Id_token))
                throw new Exception("Could not get token from Google");

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
                    throw new Exception("Failed to create user");

                await _userManager.AddToRoleAsync(user, "Customer");
            }
            var userResponse = await GetUser(user.UserName);
            return userResponse;
        }

        #endregion

        #region Password Methods
        public async Task ChangePassword(string userName, ChangePasswordDTO changePasswordDTO)
        {
            var user = await _userManager.FindByNameAsync(userName);
            bool checkResult = await _userManager.CheckPasswordAsync(user, changePasswordDTO.CurrentPassword);
            if (!checkResult)
                throw new Exception("Wrong password ! Try again.");

            if (string.Compare(changePasswordDTO.NewPassword, changePasswordDTO.ConfirmedNewPassword) != 0)
                throw new Exception("New Password and ConfirmedPassword must be the same !");

            var result = await _userManager.ChangePasswordAsync(user, changePasswordDTO.CurrentPassword, changePasswordDTO.NewPassword);

            if (!result.Succeeded)
            {
                var errors = new List<string>();

                foreach (var error in result.Errors)
                {
                    errors.Add(error.Description);
                }
                throw new Exception(string.Join(", ", errors));
            }
        }

        public async Task ForgetPassword(ForgetPasswordDTO forgetPasswordDTO)
        {
            var user = await _userManager.FindByEmailAsync(forgetPasswordDTO.Email);
            
            // Don't reveal if the email exists or not
            if (user != null)
            {   
                // Generate Token (Encode in query string)
                var token = CF.Base64ForUrlEncode(await _userManager.GeneratePasswordResetTokenAsync(user));

                string linkResetPassword = $"http://localhost:3000/get-reset-password?email={forgetPasswordDTO.Email}&token={token}";

                string htmlContent = $@"
                    <h2>Password Reset Request</h2>
                    <p>Hello,</p>
                    <p>We received a request to reset your password. If you didn't make this request, you can safely ignore this email.</p>
                    <p>To reset your password, click the link below:</p>
                    <p><a href='{linkResetPassword}'>Reset Password</a></p>
                    <p>This link will expire in 24 hours.</p>
                    <p>Best regards,<br/>ROGER BMT Store Team</p>";

                // Send Message to email
                await _emailSenderService.SendEmailAsync(forgetPasswordDTO.Email, "Reset Password - ROGER BMT Store", htmlContent);
            }

            // Always return success to prevent email enumeration
            return;
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

        public async Task ResetPassword(ResetPasswordDTO resetPasswordDTO)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordDTO.Email);

            if (user == null)
                throw new Exception("Email does not exist.");

            // Compare Password
            if (string.Compare(resetPasswordDTO.NewPassword, resetPasswordDTO.ConfirmNewPassword) != 0)
                throw new Exception("New Password and ConfirmedPassword must be the same !");

            // Handle Result
            var result = await _userManager.ResetPasswordAsync(user, CF.Base64ForUrlDecode(resetPasswordDTO.Token), resetPasswordDTO.NewPassword);
            if (!result.Succeeded)
            {
                var errors = new List<string>();
                foreach (var error in result.Errors)
                    errors.Add(error.Description);

                throw new Exception(string.Join(", ", errors));
            }
        }

        #endregion

        #region Methods

        public async Task<string> UpdateUser(UserDTO model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            await _unitOfWork.BeginTransactionAsync(TransactionType.EntityFramework);
            try
            {
                // Header
                if (model.FullName != user.FullName && !string.IsNullOrEmpty(model.FullName))
                    user.FullName = model.FullName;
                if (model.Email != user.Email && !string.IsNullOrEmpty(model.Email))
                    user.Email = model.Email;
                if (model.PhoneNumber != user.PhoneNumber && !string.IsNullOrEmpty(model.PhoneNumber))
                    user.PhoneNumber = model.PhoneNumber;
                if (model.Dob != user.Dob)
                    user.Dob = model.Dob;

                if(model.ImageUrl != null)
                {
                    user.ImageUrl = model.ImageUrl.ToString();
                    user.PublicId = model.ImageUrl.ToString();
                }
                
                // User Shipping Adress
                var userAddresses = await _unitOfWork.UserAddress.GetAllAsync(e => e.UserId == user.Id);
                if(userAddresses != null && userAddresses.Any())
                {
                    _unitOfWork.UserAddress.RemoveRangeAsync(userAddresses);
                }

                await _unitOfWork.UserAddress.AddRangeAsync(model.UserAddresses.Select(e => new UserAddress
                {
                    UserId = user.Id,
                    ShippingAddress = new ShippingAddress
                    {
                        City = e.City,
                        District = e.District,
                        Ward = e.Ward,
                        PostalCode = e.PostalCode,
                        StreetAddress = e.StreetAddress,
                        Country = e.Country,
                    }
                }));

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();    
                return model.UserName;
            }
            catch(Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw new Exception(ex.Message);
            }
        }

        #endregion
    }
}
