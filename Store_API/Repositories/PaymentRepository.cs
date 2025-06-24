using Microsoft.EntityFrameworkCore;
using Store_API.Data;
using Store_API.Enums;
using Store_API.IRepositories;
using Store_API.Models;
using Stripe;

namespace Store_API.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly StoreContext _db;
        public PaymentRepository(StoreContext db)
        {
            _db = db;
        }
        public async Task AddAsync(Payment payment)
        {
            await _db.Payments.AddAsync(payment);
        }

        public void Delete(Payment payment)
        {
            _db.Payments.Remove(payment);
        }

        public async Task<Payment> GetByPaymentIntent(string paymentIntentId)
        {
            var payment = await _db.Payments.FirstOrDefaultAsync(p => p.PaymentIntentId == paymentIntentId);
            return payment;
        }

        public async Task<Payment> GetLatestPendingPaymentAsync(int userId)
        {
            return await _db.Payments
                        .Where(p => p.UserId == userId && p.Status == PaymentStatus.Pending)
                        .OrderByDescending(p => p.CreatedAt)
                        .FirstOrDefaultAsync();
        }
    }
}
