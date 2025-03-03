using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Store_API.DTOs.Orders;
using Store_API.IService;
using Store_API.Models;
using Store_API.Repositories;

namespace Store_API.Controllers
{
    [Route("api/user-address")]
    [ApiController]
    public class UserAddressController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        public UserAddressController(IUnitOfWork unitOfWork, UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        [HttpGet("get-user-address")]
        [Authorize]
        public async Task<IActionResult> GetUserAddresses()
        {
            int userId = (await _userManager.FindByNameAsync(User.Identity.Name)).Id;
            var result = await _unitOfWork.UserAddress.GetUserAddresses(userId);
            return Ok(result.Data);
        }

        [HttpPost("upsert-user-address")]
        [Authorize]
        public async Task<IActionResult> UpsertUserAddress([FromForm] UserAddressDTO userAddressDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            int userId = (await _userManager.FindByNameAsync(User.Identity.Name)).Id;
            var result = await _unitOfWork.UserAddress.UpsertUserAddresses(userId, userAddressDTO);

            if (!result.IsSuccess)
                return BadRequest(new ProblemDetails { Title = result.Errors[0] });

            return Ok(result.Data);
        }

        [HttpDelete("delete-user-address")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _unitOfWork.UserAddress.Delete(id);

            if (!result.IsSuccess) return BadRequest(new ProblemDetails { Title = result.Errors[0] });
            return Ok(result.Data);
        }
    }
}
