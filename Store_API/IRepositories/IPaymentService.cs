using Store_API.DTOs.Baskets;
using Store_API.Models.OrderAggregate;
using Stripe;

namespace Store_API.Repositories
{
    public interface IPaymentService
    {
        Task<PaymentIntent> CreatePaymentIntentAsync(BasketDTO basket, ShippingAddress shippingAddress, bool isSaveAddress);
        Task HandleStripeWebhookAsync(Event stripeEvent);
    }
}
