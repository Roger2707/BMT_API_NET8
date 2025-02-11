using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Store_API.DTOs.Orders;
using Store_API.Models;
using Store_API.Repositories;

namespace Store_API.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        public OrdersController(IUnitOfWork unitOfWork, UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        [Authorize]
        [HttpPost("create-order")]
        public async Task<IActionResult> Create([FromForm] UserAddressDTO userAddress)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            int userId = user.Id;
            var basket = await _unitOfWork.Basket.GetBasket(User.Identity.Name);

            if (basket == null || basket.Items.Count == 0) return BadRequest(new ProblemDetails { Title = "There are no items in basket!" });

            try
            {
                _unitOfWork.BeginTrans();
                await _unitOfWork.Order.Create(userId, userAddress, basket);
                _unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return BadRequest(new ProblemDetails { Title = ex.Message });
            }
            return Ok("Order Successfully !");
        }
    }
}
