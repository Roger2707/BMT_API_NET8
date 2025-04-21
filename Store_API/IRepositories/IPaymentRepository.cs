using Store_API.Models;

namespace Store_API.IRepositories
{
    public interface IPaymentRepository
    {
        Task AddAsync(Payment payment);
        Task<Payment> GetByPaymentIntent(string paymentIntentId);
    }
}
