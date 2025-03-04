using Store_API.Models;

namespace Store_API.IRepositories
{
    public interface IPaymentRepository
    {
        Task AddAsync(Payment payment);
        Task UpdatePaymentAsync(Payment payment);
        Task<Payment> GetByPaymentIntent(string paymentIntentId);
        Task<List<Payment>> GetByOrderId(int orderId);
    }
}
