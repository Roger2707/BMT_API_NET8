using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store_API.Helpers;
using Store_API.IService;
using System.Security.Claims;

namespace Store_API.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [Authorize]
        [HttpGet("get-order-by-id")]
        public async Task<IActionResult> GetOrder(Guid orderId)
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
            var orders = await _orderService.GetOrders(CF.GetInt(User.FindFirstValue(ClaimTypes.NameIdentifier)));
            return Ok(orders);
        }
    }
}
