using Store_API.Models;

namespace Store_API.IRepositories
{
    public interface IPaymentRepository
    {
        Task<Payment> GetLatestPendingPaymentAsync(int userId);
        Task AddAsync(Payment payment);
        void Delete(Payment payment);
        Task<Payment> GetByPaymentIntent(string paymentIntentId);
    }
}
