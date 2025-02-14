using Store_API.DTOs.Baskets;
using Store_API.Models;
using Stripe;

namespace Store_API.Repositories
{
    public interface IPaymentService
    {
        public Task<PaymentIntent> UpsertPaymentIntent(BasketDTO basket);
        public Task HandleWebHook(string json, string stripeSignatureHeaders);
    }
}
