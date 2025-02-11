using Store_API.Data;
using Store_API.DTOs.Baskets;
using Store_API.Helpers;
using Store_API.Models;
using Store_API.Repositories;
using Stripe;

namespace Store_API.Services
{
    public class PaymentService : IPaymentRepository
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
    }
}
