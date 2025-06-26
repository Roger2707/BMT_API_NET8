using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store_API.DTOs.Orders;
using Store_API.Helpers;
using Store_API.Services.IService;
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
        [HttpGet("get-orders")]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orderService.GetOrders();
            return Ok(orders);
        }

        [Authorize]
        [HttpGet("get-orders-of-user")]
        public async Task<IActionResult> GetOrders()
        {
            var orders = await _orderService.GetOrders(CF.GetInt(User.FindFirstValue(ClaimTypes.NameIdentifier)));
            return Ok(orders);
        }

        [HttpPost("update-order-status")]
        public async Task<IActionResult> UpdateOrderStatus([FromBody] OrderUpdatStatusRequest request)
        {
            try
            {
                await _orderService.UpdateOrderStatus(request);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
