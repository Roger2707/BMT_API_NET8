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
        [HttpGet("get-orders-of-user")]
        public async Task<IActionResult> GetOrders()
        {
            var orders = await _orderService.GetOrders(CF.GetInt(User.FindFirstValue(ClaimTypes.NameIdentifier)));
            return Ok(orders);
        }
    }
}
