using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store_API.DTOs.Orders;
using Store_API.Helpers;
using Store_API.Services.IService;
using Stripe;
using System.Security.Claims;
using Store_API.DTOs.Payments;
using Store_API.Enums;

namespace Store_API.Controllers
{
    [Route("api/payments")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IBasketService _basketService;
        private readonly IConfiguration _configuration;

        public PaymentsController(IPaymentService paymentService, IBasketService basketService, IConfiguration configuration)
        {
            _paymentService = paymentService;
            _basketService = basketService;
            _configuration = configuration;
        }

        [Authorize]
        [HttpPost("create-client-secret")]
        public async Task<IActionResult> CreateClientSecret([FromBody] ShippingAddressDTO shippingAddressDTO)
        {
            int userId = CF.GetInt(User.FindFirstValue(ClaimTypes.NameIdentifier));
            await _paymentService.CreatePaymentAsync(userId, User.Identity.Name, shippingAddressDTO);
            return Ok(new PaymentProcessingResponse
            {
                Status = PaymentStatus.Pending,
                Message = "Payment request is being processed. You will receive updates via SignalR."
            });
        }

        // stripe listen --forward-to localhost:5110/api/payments/webhook
        [AllowAnonymous]
        [HttpPost("webhook")]
        public async Task<IActionResult> StripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var whSecret = _configuration["Stripe:WhSecret"];
            var stripeEvent = EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], whSecret);
            await _paymentService.HandleStripeWebhookAsync(stripeEvent);

            return Ok(new { Title = "Check out Successfully !" });
        }
    }
}
