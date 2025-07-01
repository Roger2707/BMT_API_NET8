using Store_API.DTOs.Baskets;
using Store_API.DTOs.Orders;
using Stripe;

namespace Store_API.Services.IService
{
    public interface IPaymentService
    {
        Task CreatePaymentAsync(int userId, string username, ShippingAddressDTO shippingAddressDTO);
        Task<PaymentIntent> CreatePaymentIntentAsync(int userId, Guid requestId, List<BasketItemDTO> items, ShippingAddressDTO shippingAddress);
        Task HandleStripeWebhookAsync(Event stripeEvent);
    }
}
