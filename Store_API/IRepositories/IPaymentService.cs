using Store_API.Models;
using Stripe;

namespace Store_API.Repositories
{
    public interface IPaymentService
    {
        Task<PaymentIntent> CreatePaymentIntentAsync(int orderId, double amount);
        Task HandleStripeWebhookAsync(Event stripeEvent);
    }
}
