using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Store_API.IService;
using Store_API.Models.Users;

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
        public async Task<IActionResult> Create(int userAddressId)
        {
            try
            {
                // 1. Check userAddressId
                if(userAddressId == 0) return BadRequest(new ProblemDetails { Title = "User Address is not found" });

                // 2. Get Basket - Current User 
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                int userId = user.Id;

                var basketDTO = await _basketService.GetBasketDTORedis(userId, User.Identity.Name);
                if (basketDTO == null) return BadRequest(new ProblemDetails { Title = "Basket is empty !" });

                // 3. Order processing
                var orderResponse = await _orderService.Create(userId, User.Identity.Name, basketDTO, userAddressId);
                return Ok(orderResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(new ProblemDetails { Title = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("get-order")]
        public async Task<IActionResult> GetOrder(int orderId)
        {
            var order = await _orderService.GetOrder(orderId);
            if (order == null) return NotFound(new ProblemDetails { Title = "Order not found" });
            return Ok(order);
        }

        [Authorize]
        [HttpGet("get-orders")]
        public async Task<IActionResult> GetOrders()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            var orders = await _orderService.GetOrders(user.Id);
            return Ok(orders);
        }

        #region Helpers

        [HttpPost("update-order-status")]
        public async Task<IActionResult> UpdateOrderStatus(int orderId)
        {
            await _orderService.UpdateOrderStatus(orderId, Models.OrderAggregate.OrderStatus.Completed);
            return Ok();
        }

        #endregion 
    }
}
