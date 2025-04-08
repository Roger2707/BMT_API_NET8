using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Store_API.DTOs.Accounts;
using Store_API.DTOs.User;
using Store_API.IService;
using Store_API.Models.Users;

namespace Store_API.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly SignInManager<User> _signInManager;
        private readonly IUserService _userService;

        public UserController(SignInManager<User> signInManager, IUserService userService)
        {
            _signInManager = signInManager;
            _userService = userService;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAll();
            if (users == null || users.Count == 0) return BadRequest(new ProblemDetails { Title = "There are no users in DB !" });
            return Ok(users);
        }

        [HttpPost("external-login")]
        public async Task<IActionResult> ExternalLogin([FromBody] GoogleAuthRequest model)
        {
            try
            {
                var userResponse = await _userService.ExternalLoginPopUp(model);
                if (userResponse != null) return Ok(userResponse);
                return BadRequest(new ProblemDetails { Title = "Login failed !" });
            }
            catch(Exception ex)
            {
                return BadRequest(new ProblemDetails { Title = ex.Message });
            }
        }

        [HttpPost("sign-up")]
        public async Task<IActionResult> SignUp(SignUpRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var userResult = await _userService.CreateUserAsync(request);
                if (userResult == null) return BadRequest();

                await _userService.SendWelcomeEmailAsync(userResult.Email, userResult.UserName, "GeneratedPassword");
                return Ok("User is created successfully!");
            }
            catch(Exception ex)
            {
                return BadRequest(new ProblemDetails { Title = ex.Message });
            }
        }

        [HttpPost("log-in")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var result = await _userService.SignInAsync(request);
                if (result == null) return BadRequest(new ProblemDetails { Title = "Login failed !" });
                return Ok(result);
            }
            catch(Exception ex)
            {
                return BadRequest(new ProblemDetails { Title = ex.Message });
            }
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
            try
            {
                await _userService.ChangePassword(User.Identity.Name, passwordDTO);
                return Ok();
            }
            catch(Exception ex)
            {
                return BadRequest(new ProblemDetails { Title = ex.Message });
            }
        }

        [HttpPost("forget-password")]
        public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordDTO model)
        {
            if(!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                await _userService.ForgetPassword(model);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new ProblemDetails { Title = ex.Message });
            }
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO model)
        {
            try
            {
                await _userService.ResetPassword(model);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new ProblemDetails { Title = ex.Message });
            }
        }

        [HttpGet("current-user")]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                string userName = User.Identity.Name;
                if (string.IsNullOrEmpty(userName)) return Ok();
                var user = await _userService.GetUser(userName);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(new ProblemDetails { Title = ex.Message });
            }
        }

        [HttpPut("update-profile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromBody] UserUpsertDTO userUpsertDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                string username = await _userService.UpdateUser(userUpsertDTO);
                var user = await _userService.GetUser(username);
                return Ok(user);
            }
            catch(Exception ex)
            {
                return BadRequest(new ProblemDetails { Title = ex.Message });
            }
        }
    }
}
