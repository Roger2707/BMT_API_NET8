using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Store_API.Data;
using Store_API.DTOs.Baskets;
using Store_API.DTOs.Orders;
using Store_API.Models;
using Store_API.Models.OrderAggregate;
using Store_API.Repositories;
using Store_API.Services;
using Stripe;
using System.Diagnostics;

namespace Store_API.Controllers
{
    [Route("api/payments")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly IRabbitMQRepository _rabbitMQService;
        public PaymentsController(IUnitOfWork unitOfWork, UserManager<User> userManager, IRabbitMQRepository rabbitMQService)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _rabbitMQService = rabbitMQService;
        }

        [Authorize]
        [HttpPost("upsert-payment-intent")]
        public async Task<IActionResult> UpsertPaymentIntent([FromForm] UserAddressDTO userAddress)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            int userId = user.Id;

            var basket = await _unitOfWork.Basket.GetBasket(User.Identity.Name);
            if(basket == null) return BadRequest(new ProblemDetails { Title = "There are no items waiting for payments !" });

            var intent = await _unitOfWork.Payment.UpsertPaymentIntent(basket);
            if (intent == null) return BadRequest(new ProblemDetails { Title = "Problem creating payment intent" });

            var paymentIntentId = basket.PaymentIntentId ?? intent.Id;
            var clientSecret = basket.ClientSecret ?? intent.ClientSecret;

            try
            {
                await _unitOfWork.Basket.UpdateBasketPayment(paymentIntentId, clientSecret, User.Identity.Name);
                
                var order = await _unitOfWork.Order.Create(userId, userAddress, basket);
                await _rabbitMQService.PublishMessage(order);
            }
            catch (Exception ex)
            {
                return BadRequest(new ProblemDetails { Title = ex.Message });
            }

            return Ok(new { clientSecret = clientSecret });
        }

        [HttpPost("trigger-payment-success")]
        public IActionResult TriggerPaymentSuccess()
        {
            var processInfo = new ProcessStartInfo
            {
                FileName = "stripe",
                Arguments = "trigger payment_intent.succeeded",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            Process.Start(processInfo);
            return Ok("🔄 Webhook event payment_intent.succeeded triggered.");
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> HandleWebhook()
        {
            string json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            string stripeSignatureHeaders = Request.Headers["Stripe-Signature"];

            try
            {
                await _unitOfWork.Payment.HandleWebHook(json, stripeSignatureHeaders);   
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest($"Webhook Error: {e.Message}");
            }
        }

    }
}
