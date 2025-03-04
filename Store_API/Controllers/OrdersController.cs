using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Store_API.DTOs.Orders;
using Store_API.IService;
using Store_API.Models;

namespace Store_API.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IBasketService _basketService;
        private readonly UserManager<User> _userManager;

        public OrdersController(IOrderService orderService, IBasketService basketService, UserManager<User> userManager)
        {
            _orderService = orderService;
            _basketService = basketService;
            _userManager = userManager;
        }

        [Authorize]
        [HttpPost("create-order")]
        public async Task<IActionResult> Create([FromForm] UserAddressDTO? userAddress, int userAddressId)
        {
            // 1. Get Basket - Current User 
            var basketDTO = await _basketService.GetBasket(User.Identity.Name);
            if (basketDTO == null) return BadRequest(new ProblemDetails { Title = "Basket is empty" });

            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            int userId = user.Id;

            // 2. Order processing
            var orderId = await _orderService.Create(userId, basketDTO, userAddressId, userAddress);

            if(orderId <= 0) return BadRequest(new ProblemDetails { Title = "Problem creating order" });
            return Ok(orderId);
        }

        [Authorize]
        [HttpGet("get-order")]
        public async Task<IActionResult> GetOrder(int orderId)
        {
            var order = await _orderService.GetOrder(orderId);
            if (order == null) return NotFound(new ProblemDetails { Title = "Order not found" });
            return Ok(order);
        }
    }
}
