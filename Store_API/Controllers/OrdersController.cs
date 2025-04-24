using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Store_API.DTOs.Orders;
using Store_API.IService;
using Store_API.Models.Users;

namespace Store_API.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly UserManager<User> _userManager;

        public OrdersController(IOrderService orderService, UserManager<User> userManager)
        {
            _orderService = orderService;
            _userManager = userManager;
        }

        [Authorize]
        [HttpPost("create-order")]
        public async Task<IActionResult> Create(OrderCreateRequest orderCreateRequest)
        {
            try
            {
                var orderResponse = await _orderService.Create(orderCreateRequest);
                return Ok(orderResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(new ProblemDetails { Title = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("get-order-by-id")]
        public async Task<IActionResult> GetOrder(int orderId)
        {
            var order = await _orderService.GetOrder(orderId);
            if (order == null) return BadRequest(new ProblemDetails { Title = "Order not found" });
            return Ok(order);
        }

        [Authorize]
        [HttpGet("get-order-by-client-secret")]
        public async Task<IActionResult> GetOrder(string clientSecret)
        {
            var order = await _orderService.GetOrder(clientSecret);
            if (order == null) return BadRequest(new ProblemDetails { Title = "Order not found" });
            return Ok(order);
        }

        [Authorize]
        [HttpGet("get-orders-of-user")]
        public async Task<IActionResult> GetOrders()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var orders = await _orderService.GetOrders(user.Id);
            return Ok(orders);
        }
    }
}
