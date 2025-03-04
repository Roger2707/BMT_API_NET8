using Microsoft.AspNetCore.Mvc;
using Store_API.Repositories;
using Stripe;

namespace Store_API.Controllers
{
    [Route("api/payments")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private IPaymentService _paymentService;
        private IConfiguration _configuration;

        public PaymentsController(IPaymentService paymentService, IConfiguration configuration)
        {
            _paymentService = paymentService;
            _configuration = configuration;
        }

        [HttpPost("create-intent/{orderId}")]
        public async Task<IActionResult> CreatePaymentIntent(int orderId, [FromBody] decimal amount)
        {
            var intent = await _paymentService.CreatePaymentIntentAsync(orderId, amount);
            return Ok(new { clientSecret = intent.ClientSecret });
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> StripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var whSecret = _configuration["StripeSettings:WhSecret"];

            var stripeEvent = EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], whSecret);
            await _paymentService.HandleStripeWebhookAsync(stripeEvent);
            return Ok();
        }
    }
}
