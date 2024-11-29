using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Store_API.Data;
using Store_API.DTOs.Accounts;
using Store_API.Helpers;
using Store_API.Models;
using Store_API.Repositories;
using Store_API.Services;
using System.Security.Claims;

namespace Store_API.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly StoreContext _db;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ITokenRepository _tokenService;

        public AccountController(IUnitOfWork unitOfWork, StoreContext db, UserManager<User> userManager, SignInManager<User> signInManager, ITokenRepository tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _db = db;
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
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
                return BadRequest(new ProblemDetails { Title = "Failed to create user." });
            }
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
            finally
            {
                _unitOfWork.CloseConnection();
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
        public async Task<IActionResult> ChangePassword(ChangePasswordDTO passwordDTO)
        {
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
            return Ok();
        }

        [HttpPost("forget-password")]
        public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordDTO model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null) return BadRequest(new ProblemDetails { Title = "Email is UnValid !" });

            string error = "";
            try
            {
                await _unitOfWork.Account.HandleForgetPassword(model, user);
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }

            if (error != "") return BadRequest(new ProblemDetails { Title = error });
            return Ok(new {message = "Check your Email to get link reset password !"});
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null) return NotFound();
            if (string.Compare(model.NewPassword, model.ConfirmNewPassword) != 0)
                return BadRequest(new ProblemDetails { Title = "Password and ConfirmPassword is not match !" });

            model.Token = CF.Base64ForUrlDecode(model.Token);

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);

            if (!result.Succeeded)
            {
                var errors = new List<string>();
                foreach (var error in result.Errors)
                    errors.Add(error.Description);

                return BadRequest(new ProblemDetails { Title = string.Join(", ", errors) });
            }
            return Ok();
        }

        [HttpGet("current-user")]
        public async Task<IActionResult> GetCurrentUser()
        {
            string userName = User.Identity.Name;
            if (string.IsNullOrEmpty(userName)) return Ok();

            var user = await _userManager.FindByNameAsync(userName);

            var respone = new LoginResponse
            {
                FullName = user.FullName,
                Email = user.Email,
                Token = await _tokenService.GenerateToken(user),
            };

            return Ok(respone);
        }
    }
}
