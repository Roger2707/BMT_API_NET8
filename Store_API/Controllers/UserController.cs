using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store_API.DTOs.Accounts;
using Store_API.DTOs.User;
using Store_API.Services.IService;

namespace Store_API.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize(Roles = "SuperAdmin, Admin")]
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
            var userResponse = await _userService.LoginOAuth(model);
            return Ok(userResponse);
        }

        [HttpPost("sign-up")]
        public async Task<IActionResult> SignUp(SignUpRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userResult = await _userService.CreateUserAsync(request);
            if (userResult == null) return BadRequest(new ProblemDetails { Title = "Sign up failed !"});

            await _userService.SendEmailLoginAsync(userResult.Email, userResult.Username);
            return Ok("User is created successfully!");
        }

        [HttpPost("log-in")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _userService.SignInAsync(request);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDTO passwordDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            await _userService.ChangePassword(User.Identity.Name, passwordDTO);
            return Ok(new { Title = "Change Password Successfully !" });
        }

        [HttpPost("forget-password")]
        public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordDTO model)
        {
            if(!ModelState.IsValid) return BadRequest(ModelState);

            await _userService.ForgetPassword(model);
            return Ok();
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO model)
        {
            await _userService.ResetPassword(model);
            return Ok(new { Title = "Reset Password Successfully !" });
        }

        [Authorize]
        [HttpGet("current-user")]
        public async Task<IActionResult> GetCurrentUser()
        {
            string userName = User.Identity.Name;
            var user = await _userService.GetUser(userName);
            return Ok(user);
        }

        [Authorize]
        [HttpPut("update-profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UserDTO userUpsertDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            await _userService.UpdateUser(userUpsertDTO);
            var user = await _userService.GetUser(userUpsertDTO.UserName);
            return Ok(user);
        }
    }
}
