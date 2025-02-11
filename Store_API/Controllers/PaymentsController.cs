using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Store_API.Data;
using Store_API.DTOs.Baskets;
using Store_API.Models;
using Store_API.Models.OrderAggregate;
using Store_API.Repositories;
using Stripe;
using System.Diagnostics;

namespace Store_API.Controllers
{
    [Route("api/payments")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IPaymentRepository _paymentService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly StoreContext _db;
        public PaymentsController(IConfiguration configuration, IPaymentRepository paymentService, IUnitOfWork unitOfWork, StoreContext db)
        {
            _configuration = configuration;
            _paymentService = paymentService;
            _unitOfWork = unitOfWork;
            _db = db;
        }

        [Authorize]
        [HttpPost("upsert-payment-intent")]
        public async Task<IActionResult> UpsertPaymentIntent()
        {
            var basket = await _unitOfWork.Basket.GetBasket(User.Identity.Name);
            if(basket == null) return BadRequest(new ProblemDetails { Title = "There are no items waiting for payments !" });

            var intent = await _paymentService.UpsertPaymentIntent(basket);
            if (intent == null) return BadRequest(new ProblemDetails { Title = "Problem creating payment intent" });

            var paymentIntentId = basket.PaymentIntentId ?? intent.Id;
            var clientSecret = basket.ClientSecret ?? intent.ClientSecret;
            try
            {
                _unitOfWork.BeginTrans();
                await _unitOfWork.Basket.UpdateBasketPayment(paymentIntentId, clientSecret, User.Identity.Name);
                _unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
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
        public async Task<IActionResult> StripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            try
            {
                var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    Request.Headers["Stripe-Signature"],
                    _configuration["StripeSettings:WhSecret"]
                );

                if (stripeEvent.Type == Events.PaymentIntentSucceeded)
                {
                    var paymentIntent = stripeEvent.Data.Object as PaymentIntent;

                    // 🔹 Tìm đơn hàng có PaymentIntentId khớp
                    var order = await _db.Orders.FirstOrDefaultAsync(o => o.PaymentIntentId == paymentIntent.Id);

                    if (order != null)
                    {
                        order.Status = OrderStatus.Shipped; // Cập nhật trạng thái đơn hàng
                        await _db.SaveChangesAsync();
                        Console.WriteLine($"✅ Đơn hàng {order.Id} đã cập nhật trạng thái SHIPPED.");
                    }
                }
                return Ok();
            }
            catch (StripeException e)
            {
                return BadRequest($"Webhook Error: {e.Message}");
            }
        }

    }
}
