using Store_API.DTOs.Baskets;
using Store_API.Models;
using Stripe;

namespace Store_API.Repositories
{
    public interface IPaymentRepository
    {
        public Task<PaymentIntent> UpsertPaymentIntent(BasketDTO basket);
    }
}
