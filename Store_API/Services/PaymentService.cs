using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Store_API.Data;
using Store_API.DTOs.Baskets;
using Store_API.Helpers;
using Store_API.Models;
using Store_API.Models.OrderAggregate;
using Store_API.Repositories;
using Stripe;

namespace Store_API.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IConfiguration _config;
        private readonly StoreContext _db;
        public PaymentService(IConfiguration config, StoreContext db)
        {
            _config = config;
            _db = db;
        }

        public async Task<PaymentIntent> UpsertPaymentIntent(BasketDTO basket)
        {
            StripeConfiguration.ApiKey = _config["StripeSettings:SecretKey"];
            var service = new PaymentIntentService();
            var intent = new PaymentIntent();
            var subTotal = basket.Items.Where(item => item.Status == true).Sum(item => item.DiscountPrice * item.Quantity);
            var deliveryFee = subTotal > 100 ? 0 : 10;
            var amount = subTotal + deliveryFee;

            if(string.IsNullOrEmpty(basket.PaymentIntentId))
            {
                var options = new PaymentIntentCreateOptions
                {
                    Amount = CF.GetLong(amount),
                    Currency = "usd",
                    PaymentMethodTypes = new List<string>() { "card" },
                };
                intent = await service.CreateAsync(options);
            }
            else
            {
                var options = new PaymentIntentUpdateOptions
                {
                    Amount = CF.GetLong(amount),
                };
                await service.UpdateAsync(basket.PaymentIntentId, options);
            }
            return intent;
        }

        public async Task HandleWebHook(string json, string stripeSignatureHeaders)
        {
            try
            {
                var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    stripeSignatureHeaders,
                    _config["StripeSettings:WhSecret"]
                );

                if (stripeEvent.Type == Events.PaymentIntentSucceeded)
                {
                    var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                    var order = await _db.Orders.FirstOrDefaultAsync(o => o.PaymentIntentId == paymentIntent.Id);

                    if (order != null)
                    {
                        order.Status = OrderStatus.Shipped; 
                        await _db.SaveChangesAsync();
                        //await _hubContext.Clients.All.SendAsync("OrderStatusUpdated", order.Id, order.Status);
                    }
                }
                else if(stripeEvent.Type == Events.PaymentIntentPaymentFailed)
                {

                }
            }
            catch(StripeException ex)
            {
                throw new Exception(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
