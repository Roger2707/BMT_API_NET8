using Microsoft.EntityFrameworkCore;
using Store_API.Data;
using Store_API.Enums;
using Store_API.Infrastructures;
using Store_API.Models;
using Store_API.Repositories.IRepositories;

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
                        .OrderByDescending(p => p.CreatedAt)
                        .FirstOrDefaultAsync(p => p.UserId == userId && p.Status == PaymentStatus.Pending);
        }
    }
}
