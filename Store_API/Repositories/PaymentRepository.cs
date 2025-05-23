﻿using Microsoft.EntityFrameworkCore;
using Store_API.Data;
using Store_API.IRepositories;
using Store_API.Models;

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

        public async Task<Payment> GetByPaymentIntent(string paymentIntentId)
        {
            var payment = await _db.Payments.FirstOrDefaultAsync(p => p.PaymentIntentId == paymentIntentId);
            return payment;
        }

    }
}
