using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Store_API.Data;
using Store_API.DTOs.Baskets;
using Store_API.Models;
using Store_API.Models.Order;
using Store_API.Repositories;
using Stripe;

namespace Store_API.Controllers
{
    [Route("api/[controller]/[action]")]
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

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UpsertPaymentIntent()
        {
            var basket = await _unitOfWork.Basket.GetBasket(User.Identity.Name);
            var intent = await _paymentService.UpsertPaymentIntent(basket);
            if (intent == null) return BadRequest(new ProblemDetails { Title = "Problem creating payment intent" });
            var paymentIntentId = basket.PaymentIntentId ?? intent.Id;
            var clientSecret = basket.ClientSecret ?? intent.ClientSecret;

            string error = "";
            try
            {
                _unitOfWork.BeginTrans();

                await _unitOfWork.Basket.UpdateBasketPayment(paymentIntentId, clientSecret, User.Identity.Name);

                _unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                error = ex.Message;
                _unitOfWork.Rollback();
            }
            finally
            {
                _unitOfWork.CloseConnection();
            }

            if (error != "") return BadRequest(new ProblemDetails { Title = "Problem updating basket with intent" });
            var updatedBasket = await _unitOfWork.Basket.GetBasket(User.Identity.Name);
            return Ok(updatedBasket);
        }

        [HttpPost("webhook")]
        public async Task<ActionResult> StripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var stripeEvent = EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], _configuration["StripeSettings:WhSecret"]);
            var charge = (Charge)stripeEvent.Data.Object;
            
            var order = await _db.Orders.FirstOrDefaultAsync(o => o.PaymentIntentId == charge.PaymentIntentId);
            if(charge.Status == "succeeded") order.Status = OrderStatus.PaymentSuccess;
            await _db.SaveChangesAsync();
            return new EmptyResult();
        }
    }
}
