using Newtonsoft.Json;
using Store_API.DTOs.Accounts;
using Store_API.DTOs.User;
using Google.Apis.Auth;
using Store_API.Models.Users;
using Store_API.Enums;
using Store_API.Models.OrderAggregate;
using Store_API.Services.IService;
using Store_API.Infrastructures;

namespace Store_API.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly EmailSenderService _emailSenderService;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _configuration;
        private readonly IRedisService _redisService;

        public UserService(EmailSenderService emailSenderService, IUnitOfWork unitOfWork
            , ITokenService tokenService, IConfiguration configuration, IRedisService redisService) 
        {
            _unitOfWork = unitOfWork;
            _emailSenderService = emailSenderService;
            _tokenService = tokenService;
            _configuration = configuration;
            _redisService = redisService;
        }

        #region Retrieve

        public async Task<List<UserDTO>> GetAll()
        {
            var users = await _unitOfWork.User.GetAll();
            return users;
        }

        public async Task<UserDTO> GetUser(string userName)
        {
            var user = await _unitOfWork.User.GetUser(userName);
            var token = await _tokenService.GenerateToken(new User { Id = user.Id, Email = user.Email, Username = user.UserName });
            user.Token = token;
            return user;
        }

        public async Task<UserDTO> GetUser(int userId)
        {
            var user = await _unitOfWork.User.GetUser(userId);
            var token = await _tokenService.GenerateToken(new User { Id = user.Id, Email = user.Email, Username = user.UserName });
            user.Token = token;
            return user;
        }

        #endregion

        #region Sign Up

        public async Task<User> CreateUserAsync(SignUpRequest request)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                if (string.IsNullOrWhiteSpace(request.Username))
                    throw new ArgumentException("Username is required.");

                if (await _unitOfWork.User.FindFirstAsync(u => u.Email == request.Email) != null)
                    throw new ArgumentException("Email already exists.");

                if (await _unitOfWork.User.FindFirstAsync(u => u.Username == request.Username) != null)
                    throw new ArgumentException("Username already exists.");

                string password = request.Username[0].ToString().ToUpper() + request.Username.Substring(1).ToLower() + "@123";

                var user = new User
                {
                    FullName = request.FullName,
                    Email = request.Email,
                    Username = request.Username,
                    PhoneNumber = request.PhoneNumber,
                    PasswordHash = HashPassword(password),
                    Provider = "System",
                };

                await _unitOfWork.User.AddAsync(user);
                await _unitOfWork.SaveChangesAsync();

                var userRole = new UserRole { UserId = user.Id, RoleId = 3 };
                await _unitOfWork.UserRole.AddAsync(userRole);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();

                return user;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task SendEmailLoginAsync(string email, string username)
        {
            string content = $@"    Hello, welcome to our badminton store
                                    Your Initial info:

                                    Username: {username}
                                    Password: {username + "@123"}

                                    You should change your password after log in in order to have strong protection for your account.
                                    Have a nice day!
                                                                            [ROGER]                                       
                                ";

            await _emailSenderService.SendEmailAsync(email, "[NEW] 🔥 Your Account:", content);
        }

        #endregion

        #region Log in
        public async Task<UserDTO> SignInAsync(LoginRequest request)
        {
            var exitedUser = await _unitOfWork.User.FindFirstAsync(u => u.Username == request.Username);
            if (exitedUser == null || !VerifyPassword(request.Password, exitedUser.PasswordHash))
                throw new Exception("Username or password is incorrect.");

            var userResponse = await GetUser(exitedUser.Username);
            return userResponse;
        }

        #endregion

        #region OAUth 2.0 (Google)

        public async Task<UserDTO> LoginOAuth(GoogleAuthRequest request)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync(TransactionType.EntityFramework);

                if (string.IsNullOrEmpty(request.AuthCode))
                    throw new ArgumentNullException("Authorization code is null or empty.");

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
                    throw new HttpRequestException($"Failed to login {responseString}");

                var tokenResponse = JsonConvert.DeserializeObject<GoogleTokenResponse>(responseString);
                if (tokenResponse == null || string.IsNullOrEmpty(tokenResponse.Id_token))
                    throw new InvalidOperationException("Could not get token from Google");

                var settings = new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new[] { _configuration["OAuth:ClientID"] }
                };
                var payload = GoogleJsonWebSignature.ValidateAsync(tokenResponse.Id_token, settings).Result;
                var user = await _unitOfWork.User.FindFirstAsync(u => u.Email == payload.Email);

                if (user == null)
                {
                    user = new User { Username = payload.Email, Email = payload.Email, FullName = payload.Name, Provider = "Google" };

                    await _unitOfWork.User.AddAsync(user);
                    await _unitOfWork.SaveChangesAsync();

                    var userRole = new UserRole { UserId = user.Id, RoleId = 3 };
                    await _unitOfWork.UserRole.AddAsync(userRole);
                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.CommitAsync();
                }
                var userResponse = await GetUser(user.Username);
                return userResponse;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        #endregion

        #region Password Methods
        public async Task ChangePassword(string userName, ChangePasswordDTO changePasswordDTO)
        {
            var user = await _unitOfWork.User.FindFirstAsync(u => u.Username == userName);
            bool checkResult = VerifyPassword(changePasswordDTO.CurrentPassword, user.PasswordHash);
            if (!checkResult)
                throw new UnauthorizedAccessException("Wrong password ! Try again.");

            if (string.Compare(changePasswordDTO.NewPassword, changePasswordDTO.ConfirmedNewPassword) != 0)
                throw new ArgumentException("New Password and ConfirmedPassword must be the same !");

            user.PasswordHash = HashPassword(changePasswordDTO.NewPassword);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task ForgetPassword(ForgetPasswordDTO forgetPasswordDTO)
        {
            var user = await _unitOfWork.User.FindFirstAsync(u => u.Email == forgetPasswordDTO.Email);
            if (user != null)
            {   
                var token = Guid.NewGuid().ToString("N");
                await _redisService.SetAsync($"reset-password:{forgetPasswordDTO.Email}", token, TimeSpan.FromMinutes(1));

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
            return;
        }

        public async Task ResetPassword(ResetPasswordDTO resetPasswordDTO)
        {
            var user = await _unitOfWork.User.FindFirstAsync(u => u.Email == resetPasswordDTO.Email);
            if (user == null)
                throw new ArgumentException("Email does not exist.");

            // Check Token valid
            var token = await _redisService.GetAsync<string>($"reset-password:{resetPasswordDTO.Email}");
            if (string.IsNullOrEmpty(token) || !token.Equals(resetPasswordDTO.Token))
                throw new ArgumentException("Invalid or expired token.");

            // Compare Password
            if (string.Compare(resetPasswordDTO.NewPassword, resetPasswordDTO.ConfirmNewPassword) != 0)
                throw new ArgumentException("New Password and ConfirmedPassword must be the same !");

            user.PasswordHash = HashPassword(resetPasswordDTO.NewPassword);

            await _redisService.RemoveAsync($"reset-password:{resetPasswordDTO.Email}");
            await _unitOfWork.SaveChangesAsync();
        }

        #endregion

        #region Update / Password Helpers

        public async Task UpdateUser(UserDTO model)
        {
            var user = await _unitOfWork.User.FindFirstAsync(u => u.Username == model.UserName);
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
                var userAddresses = await _unitOfWork.UserAddress
                    .GetAllAsync(model, filter =>
                    {
                        return u => u.UserId == user.Id;
                    });

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
            }
            catch(Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw new Exception(ex.Message);
            }
        }

        private string HashPassword(string plainPassword)
        {
            return BCrypt.Net.BCrypt.HashPassword(plainPassword);
        }

        private bool VerifyPassword(string plainPassword, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(plainPassword, hashedPassword);
        }

        #endregion
    }
}
