using Google.Apis.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Store_API.DTOs.Accounts;
using Store_API.DTOs.Orders;
using Store_API.DTOs.User;
using Store_API.Helpers;
using Store_API.IService;
using Store_API.Models.Users;

namespace Store_API.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IUserService _userService;

        public UserController(UserManager<User> userManager, SignInManager<User> signInManager, IUserService userService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userService = userService;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _userService.GetAll();

            var users = result.Data;
            if (users == null || users.Count == 0) return BadRequest(new ProblemDetails { Title = "There are no users in DB !" });

            return Ok(users);
        }

        [HttpPost("external-login")]
        public async Task<IActionResult> ExternalLogin([FromBody] GoogleAuthRequest model)
        {
            try
            {
                var result = await _userService.ExternalLoginPopUp(model);
                if (result.IsSuccess)
                {
                    var userResponse = result.Data;
                    return Ok(userResponse);
                }
                return BadRequest(new ProblemDetails { Title = result.Errors[0] });
            }
            catch(Exception ex)
            {
                return StatusCode(500, new { message = "Internal Server Error", error = ex.Message });
            }
        }

        [HttpPost("sign-up")]
        public async Task<IActionResult> SignUp(SignUpRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userResult = await _userService.CreateUserAsync(request);
            if (!userResult.IsSuccess) return BadRequest(userResult.Errors);

            var roleResult = await _userService.AssignRoleAsync(userResult.Data, "Customer");
            if (!roleResult.IsSuccess) return BadRequest(roleResult.Errors);

            await _userService.SendWelcomeEmailAsync(userResult.Data.Email, userResult.Data.UserName, "GeneratedPassword");

            return Ok("User is created successfully!");
        }

        [HttpPost("log-in")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var result = await _userService.SignInAsync(request);
            if(!result.IsSuccess) return BadRequest(result.Errors);

            return Ok(result.Data);
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
            var result = await _userService.ChangePassword(User.Identity.Name, passwordDTO);

            if(!result.IsSuccess) return BadRequest(new ProblemDetails { Title = result.Errors[0] }); 

            return Ok(new {Title = result.Data });
        }

        [HttpPost("forget-password")]
        public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordDTO model)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.ForgetPassword(model);

            if (!result.IsSuccess) return BadRequest(new ProblemDetails { Title = result.Errors[0] });
            return Ok(new { message = result.Data });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO model)
        {
            var result = await _userService.ResetPassword(model);
            if (!result.IsSuccess) return BadRequest(new ProblemDetails { Title = result.Errors[0] });
            return Ok(new { message = result.Data });
        }

        [HttpGet("current-user")]
        public async Task<IActionResult> GetCurrentUser()
        {
            string userName = User.Identity.Name;
            if (string.IsNullOrEmpty(userName)) return Ok();

            var result = await _userService.GetCurrentUser(userName);

            return Ok(result.Data);
        }


        [HttpPut("update-profile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromForm] UserProfileDTO profileDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            string userName = User.Identity.Name;

            var result = await _userService.UpdateUserProfile(userName, profileDTO);
            if(!result.IsSuccess)
                return BadRequest(new ProblemDetails { Title = result.Errors[0] });

            return Ok(new { Title = result.Data });
        }
    }
}
