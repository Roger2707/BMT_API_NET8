using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Store_API.Data;
using Store_API.DTOs.Accounts;
using Store_API.DTOs.Orders;
using Store_API.Helpers;
using Store_API.Models;
using Store_API.Models.OrderAggregate;
using Store_API.Repositories;
using Store_API.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace Store_API.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ITokenRepository _tokenService;
        private readonly EmailSenderService _emailSenderService;
        private readonly IImageRepository _imageService;
        private readonly StoreContext _db;

        public AccountController(IUnitOfWork unitOfWork, UserManager<User> userManager, SignInManager<User> signInManager
            , ITokenRepository tokenService, EmailSenderService emailSenderService, IImageRepository imageService, StoreContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _emailSenderService = emailSenderService;
            _imageService = imageService;
            _db = db;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {
            var accounts = await _unitOfWork.Account.GetAll();

            if (accounts == null || accounts.Count == 0) return NotFound();

            return Ok(accounts);
        }

        [HttpGet("external-login")]
        public IActionResult ExternalLogin()
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account");
            var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
            return Challenge(properties, "Google");
        }

        [HttpGet("external-login-callback")]
        public async Task<IActionResult> ExternalLoginCallback()
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
                return BadRequest(new ProblemDetails { Title = "Error loading external login information." });

            var jsonSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.Indented
            };

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

                HttpContext.Response.Cookies.Append("user", JsonConvert.SerializeObject(userResponse, jsonSettings), new CookieOptions
                {
                    Secure = false,
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTime.UtcNow.AddHours(1)
                });

                HttpContext.Response.Headers.Append("Authorization", $"Bearer {userResponse.Token}");
                return Redirect($"http://localhost:3000/callback");
            }
            else
            {
                // If the user does not have an account, you may register them or prompt them to create one
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

                    HttpContext.Response.Cookies.Append("user", JsonConvert.SerializeObject(userResponse, jsonSettings), new CookieOptions
                    {
                        Secure = false,
                        SameSite = SameSiteMode.Lax,
                        Expires = DateTime.UtcNow.AddHours(1)
                    });

                    HttpContext.Response.Headers.Append("Authorization", $"Bearer {userResponse.Token}");
                    return Redirect($"http://localhost:3000/callback");
                }
            }
            return BadRequest();
        }

        [HttpPost("sign-up")]
        public async Task<IActionResult> SignUp(SignUpRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (await _userManager.FindByNameAsync(request.Username) != null)
                return BadRequest(new ProblemDetails { Title = "Username has existed !" });

            string error = "";
            try
            {
                _unitOfWork.BeginTrans();

                await _unitOfWork.Account.Create(request, "Customer");

                _unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                error = ex.Message;
                _unitOfWork.Rollback();
            }

            if (error != "") return BadRequest(new ProblemDetails { Title = error });
            return Ok();
        }

        [HttpPost("log-in")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var exitedUser = await _userManager.FindByNameAsync(request.Username);

            if (exitedUser == null || !await _userManager.CheckPasswordAsync(exitedUser, request.Password))
                return BadRequest(new ProblemDetails { Title = "Username or Password is incorrect !" });

            await _signInManager.SignInAsync(exitedUser, isPersistent: false);

            var userResponse = new LoginResponse
            {
                FullName = exitedUser.FullName,
                Email = exitedUser.Email,
                Token = await _tokenService.GenerateToken(exitedUser)
            };

            return Ok(userResponse);
        }

        [HttpPost("log-out")]
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            HttpContext.Response.Cookies.Delete("user");
            return Ok("Log Out Successfully !");
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordDTO passwordDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            bool checkResult = await _userManager.CheckPasswordAsync(user, passwordDTO.CurrentPassword);
            if (!checkResult)
                return BadRequest(new ProblemDetails { Title = "Password is uncorrect !" });

            if (string.Compare(passwordDTO.NewPassword, passwordDTO.ConfirmedNewPassword) != 0)
                return BadRequest(new ProblemDetails { Title = " New Password and ConfirmedPassword must be the same ! " });

            var result = await _userManager.ChangePasswordAsync(user, passwordDTO.CurrentPassword, passwordDTO.NewPassword);

            if (!result.Succeeded)
            {
                var errors = new List<string>();

                foreach (var error in result.Errors)
                {
                    errors.Add(error.Description);
                }

                return BadRequest(new ProblemDetails { Title = string.Join(", ", errors) });
            }
            return Ok(new {Title = "Successfully, Password was changed!"});
        }

        [HttpPost("forget-password")]
        public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordDTO model)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null) return BadRequest(new ProblemDetails { Title = "Email is not existed for any accounts !" });

            // Generate Token (Encode in query string)
            var token = CF.Base64ForUrlEncode(await _userManager.GeneratePasswordResetTokenAsync(user));

            string linkResetPassword = $"http://localhost:3000/get-reset-password?email={model.Email}&token={token}";
            string htmlContent = $"This is your link to reset password :" + Environment.NewLine
               + $"Link :" + linkResetPassword;

            // Send Message to email
            await _emailSenderService.SendEmailAsync(model.Email, "Reset Password ROGER BMT APP (NET 8)", htmlContent);

            return Ok(new { message = $"Link reset password is sent to your email: {model.Email}." });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO model)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                // Check user existed
                if (user == null)
                    return BadRequest(new ProblemDetails { Title = "User is not existed !"});

                // Compare Password
                if (string.Compare(model.NewPassword, model.ConfirmNewPassword) != 0)
                    return BadRequest(new ProblemDetails { Title = " Password is not matched! Try again " });

                // Handle Result
                var result = await _userManager.ResetPasswordAsync(user, CF.Base64ForUrlDecode(model.Token), model.NewPassword);
                if (!result.Succeeded)
                {
                    var errors = new List<string>();
                    foreach (var error in result.Errors)
                        errors.Add(error.Description);

                    return BadRequest(new ProblemDetails { Title = string.Join(", ", errors) });
                }

                return Ok(new {Title = "Password reset successful." });
            }
            catch
            {
                return BadRequest(new ProblemDetails { Title= "Invalid or expired token." });
            }
        }

        [HttpGet("current-user")]
        public async Task<IActionResult> GetCurrentUser()
        {
            string userName = User.Identity.Name;
            if (string.IsNullOrEmpty(userName)) return Ok();

            var user = await _userManager.FindByNameAsync(userName);

            var respone = new UserDTO
            {
                FullName = user.FullName,
                Email = user.Email,
                Dob = user.Dob,
                PhoneNumber = user.PhoneNumber,
                ImageUrl = user.ImageUrl,
                Token = await _tokenService.GenerateToken(user),
            };

            return Ok(respone);
        }


        [HttpPut("update-profile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromForm] UserProfileDTO profileDTO)
        {
            if(!ModelState.IsValid) return BadRequest(ModelState);
            
            string userName = User.Identity.Name;
            var user = await _userManager.FindByNameAsync(userName);

            // Update
            if(profileDTO.FullName != user.FullName && profileDTO.FullName != "")
                user.FullName = profileDTO.FullName;
            if(profileDTO.PhoneNumber != user.PhoneNumber && profileDTO.PhoneNumber != "")
                user.PhoneNumber = profileDTO.PhoneNumber;
            if(profileDTO.Dob != user.Dob && profileDTO.Dob.ToString() != "")
                user.Dob = profileDTO.Dob;

            if(profileDTO.ImageUrl != null)
            {
                var imageResult = await _imageService.AddImageAsync(profileDTO.ImageUrl, "accounts");

                if (imageResult.Error != null) throw new Exception(imageResult.Error.Message);

                user.ImageUrl = imageResult.SecureUrl.ToString();
                user.PublicId = imageResult.PublicId;
            }

            int result = await _unitOfWork.SaveChanges();

            if (result > 0)
            {
                var updatedUser = await _userManager.FindByNameAsync(userName);

                return Ok(new
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

            return BadRequest(new ProblemDetails { Title = "Update User Info Failed !" });
        }

        [HttpGet("get-user-address")]
        [Authorize]
        public async Task<IActionResult> GetUserAddresses()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            var userAddress = await _db.UserAddresses
                .Where(u => u.UserId == user.Id)
                .Select(u => new
                        {
                            Id = u.Id,
                            City = u.City,
                            District = u.District,
                            Ward = u.Ward,
                            PostalCode = u.PostalCode,
                            StreetAddress = u.StreetAddress,
                            Country = u.Country,
                        })
                .ToListAsync();

            return Ok(userAddress);
        }

        [HttpPost("upsert-user-address")]
        [Authorize]
        public async Task<IActionResult> CreateUserAddress([FromForm] UserAddressDTO userAddressDTO)
        {
            if(!ModelState.IsValid) return BadRequest(ModelState);
            int id = userAddressDTO.Id;
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            try
            {
                if (id == 0)
                {
                    var userAddress = new UserAddress
                    {
                        Id = userAddressDTO.Id,
                        City = userAddressDTO.City,
                        District = userAddressDTO.District,
                        Ward = userAddressDTO.Ward,
                        StreetAddress = userAddressDTO.StreetAddress,
                        PostalCode = userAddressDTO.PostalCode,
                        Country = userAddressDTO.Country,
                        UserId = user.Id,
                    };
                    await _db.UserAddresses.AddAsync(userAddress);
                }
                else
                {
                    var existeduserAddress = await _db.UserAddresses.FirstOrDefaultAsync(u => u.Id == id);
                    if(userAddressDTO.City != existeduserAddress.City)
                        existeduserAddress.City = userAddressDTO.City;
                    if (userAddressDTO.District != existeduserAddress.District)
                        existeduserAddress.District = userAddressDTO.District;
                    if (userAddressDTO.Ward != existeduserAddress.Ward)
                        existeduserAddress.Ward = userAddressDTO.Ward;
                    if (userAddressDTO.StreetAddress != existeduserAddress.StreetAddress)
                        existeduserAddress.StreetAddress = userAddressDTO.StreetAddress;
                    if (userAddressDTO.PostalCode != existeduserAddress.PostalCode)
                        existeduserAddress.PostalCode = userAddressDTO.PostalCode;
                    if (userAddressDTO.Country != existeduserAddress.Country)
                        existeduserAddress.Country = userAddressDTO.Country;
                }

                int result = await _db.SaveChangesAsync();
                if(result > 0) return Ok(new { Title = id == 0 ? "Create new address successfully !" : "Update address successfully !" });
            }
            catch (Exception ex)
            {
                return BadRequest(new ProblemDetails { Title = ex.Message });
            }
            return BadRequest(new ProblemDetails { Title = id == 0 ? "Create new address failed !" : "Update address failed !" });
        }

        [HttpDelete("delete-user-address")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var userAddress = await _db.UserAddresses.FindAsync(id);
            if (userAddress == null) return BadRequest(new ProblemDetails { Title = "Address is not existed !" });
            try
            {
                _db.UserAddresses.Remove(userAddress);
                int result = await _db.SaveChangesAsync();
                if (result > 0) return Ok(new { Title = "Delete Address Successfully !"});
            }
            catch (Exception ex)
            {
                return BadRequest(new ProblemDetails { Title = ex.Message });
            }
            return BadRequest(new ProblemDetails { Title = "Delete address failed !" });
        }
    }
}
