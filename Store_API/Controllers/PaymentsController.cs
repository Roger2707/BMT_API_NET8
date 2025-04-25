using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store_API.Helpers;
using Store_API.IService;
using Store_API.Models.OrderAggregate;
using Store_API.Repositories;
using Stripe;
using System.Security.Claims;

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

        [HttpPost("create-client-secret")]
        public async Task<IActionResult> CreateClientSecret([FromBody] ShippingAddress shippingAddress, bool isSaveAddress)
        {
            try
            {
                var basketDTO = await _basketService.GetBasketDTO(CF.GetInt(User.FindFirstValue(ClaimTypes.NameIdentifier)), User.Identity.Name);
                if (basketDTO == null) return BadRequest(new ProblemDetails { Title = "Basket is empty !" });
                var paymentIntent = await _paymentService.CreatePaymentIntentAsync(basketDTO, shippingAddress, isSaveAddress);

                return Ok(new {ClientSecret = paymentIntent.ClientSecret });
            }
            catch (Exception ex)
            {
                return BadRequest(new ProblemDetails { Title = ex.Message });
            }
        }

        // stripe listen --forward-to localhost:5110/api/payments/webhook
        [AllowAnonymous]
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
    }
}
