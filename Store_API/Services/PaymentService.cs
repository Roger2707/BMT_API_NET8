using Microsoft.AspNetCore.SignalR;
using Store_API.Helpers;
using Store_API.Hubs;
using Store_API.Models;
using Store_API.Models.OrderAggregate;
using Store_API.Repositories;
using Stripe;

namespace Store_API.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHubContext<OrderHub> _hubContext;
        private readonly IConfiguration _configuration;
        private readonly string _apiKey;

        public PaymentService(IUnitOfWork unitOfWork, IHubContext<OrderHub> hubContext, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _hubContext = hubContext;
            _apiKey = _configuration["Stripe:SecretKey"];

            if (string.IsNullOrEmpty(_apiKey)) throw new Exception("Stripe API Key is missing from configuration.");

            if (string.IsNullOrEmpty(StripeConfiguration.ApiKey))
                StripeConfiguration.ApiKey = _apiKey;
        }

        #region CRUD

        public async Task AddAsync(Payment payment)
        {
            await _unitOfWork.Payment.AddAsync(payment);
        }

        public async Task<Payment> GetByPaymentIntentIdAsync(string paymentIntentId)
        {
            return await _unitOfWork.Payment.GetByPaymentIntent(paymentIntentId);
        }

        public async Task<List<Payment>> GetPaymentsByOrderIdAsync(int orderId)
        {
            return await _unitOfWork.Payment.GetByOrderId(orderId);
        }

        #endregion

        #region Payment Methods

        public async Task<PaymentIntent> CreatePaymentIntentAsync(int orderId, double amount)
        {
            // Exchange rate (vnd - usd)
            decimal exchangeRate = 0.000039m;
            long amountInCents = (long)(CF.GetDecimal(amount) * exchangeRate * 100);

            var options = new PaymentIntentCreateOptions
            {
                Amount = amountInCents,
                Currency = "usd",
                PaymentMethodTypes = new List<string> { "card" }
            };

            var service = new PaymentIntentService();
            var paymentIntent = await service.CreateAsync(options);

            var payment = new Payment
            {
                OrderId = orderId,
                PaymentIntentId = paymentIntent.Id,
                Amount = amount,
                Status = OrderStatus.Pending,
                CreatedAt = DateTime.UtcNow,
            };

            await AddAsync(payment);

            return paymentIntent;
        }

        public async Task HandleStripeWebhookAsync(Event stripeEvent)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                if (stripeEvent.Type == Events.PaymentIntentSucceeded)
                {
                    var intent = stripeEvent.Data.Object as PaymentIntent;
                    var payment = await GetByPaymentIntentIdAsync(intent.Id);
                    var order = await _unitOfWork.Order.FirstOrDefaultAsync(payment.OrderId);

                    if (payment != null && order != null)
                    {
                        payment.Status = OrderStatus.Completed;
                        order.Status = OrderStatus.Completed;
                        await _unitOfWork.SaveChangesAsync();

                        // SignalIR
                        await _hubContext.Clients.All.SendAsync("OrderUpdated", payment.OrderId, OrderStatus.Completed);
                    }
                    else throw new Exception("Exception Webhook - Not found Payment or Order !");
                }

                await transaction.CommitAsync();
            }
            catch(Exception ex)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        #endregion
    }
}
