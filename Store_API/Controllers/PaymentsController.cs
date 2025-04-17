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

        // stripe listen --forward-to localhost:5110/api/payments/webhook
        [HttpPost("webhook")]
        public async Task<IActionResult> StripeWebhook()
        {
            try
            {
                var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
                var whSecret = _configuration["Stripe:WhSecret"];
                var stripeEvent = EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], whSecret);

                // Can improve this by using RabbitMQ
                await _paymentService.HandleStripeWebhookAsync(stripeEvent);
            }
            catch (Exception ex)
            {
                return BadRequest(new ProblemDetails { Title = ex.Message });
            }

            return Ok(new { Title = "Check out Successfully !" });
        }

        #region API test helpers

        [HttpPost("confirm-payment")]
        public async Task<IActionResult> ConfirmPayment(string paymentIntentId)
        {
            var service = new PaymentIntentService();
            var paymentIntent = await service.ConfirmAsync(paymentIntentId, new PaymentIntentConfirmOptions
            {
                PaymentMethod = "pm_card_visa"
            });

            return Ok(new
            {
                PaymentIntentId = paymentIntent.Id,
                Status = paymentIntent.Status
            });
        }


        [HttpGet("payment-status/{paymentIntentId}")]
        public async Task<IActionResult> GetPaymentStatus(string paymentIntentId)
        {
            var service = new PaymentIntentService();
            var paymentIntent = await service.GetAsync(paymentIntentId);

            return Ok(new
            {
                PaymentIntentId = paymentIntent.Id,
                Status = paymentIntent.Status
            });
        }

        #endregion
    }
}
