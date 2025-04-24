using Store_API.DTOs.Baskets;
using Stripe;

namespace Store_API.Repositories
{
    public interface IPaymentService
    {
        Task<PaymentIntent> CreatePaymentIntentAsync(BasketDTO basket, int userAddressId);
        Task HandleStripeWebhookAsync(Event stripeEvent);
    }
}
