using Microsoft.EntityFrameworkCore;
using Store_API.Data;
using Store_API.Enums;
using Store_API.IRepositories;
using Store_API.IService;
using Store_API.Models;

namespace Store_API.Repositories
{
    public class PaymentRepository : Repository<Payment>, IPaymentRepository
    {
        public PaymentRepository(StoreContext db, IDapperService dapperService) : base(db, dapperService)
        {
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
