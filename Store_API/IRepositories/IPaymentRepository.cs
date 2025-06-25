using Store_API.Models;

namespace Store_API.IRepositories
{
    public interface IPaymentRepository : IRepository<Payment>
    {
        Task<Payment> GetLatestPendingPaymentAsync(int userId);
    }
}
