using Store_API.Models;
using Stripe;

namespace Store_API.Repositories
{
    public interface IPaymentService
    {
        Task AddAsync(Payment payment);
        Task<Payment> GetByPaymentIntentIdAsync(string paymentIntentId);
        Task<List<Payment>> GetPaymentsByOrderIdAsync(int orderId);

        // 
        Task<PaymentIntent> CreatePaymentIntentAsync(int orderId, double amount);
        Task HandleStripeWebhookAsync(Event stripeEvent);
    }
}
