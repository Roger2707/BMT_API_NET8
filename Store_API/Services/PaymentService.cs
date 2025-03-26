using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Store_API.DTOs;
using Store_API.DTOs.Payments;
using Store_API.Helpers;
using Store_API.Hubs;
using Store_API.IService;
using Store_API.Models;
using Store_API.Models.OrderAggregate;
using Store_API.Repositories;
using Stripe;

namespace Store_API.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRabbitMQService _rabbitMQService;
        private readonly IConfiguration _configuration;
        private readonly string _apiKey;

        public PaymentService(IUnitOfWork unitOfWork, IRabbitMQService rabbitMQService, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _rabbitMQService = rabbitMQService;
            _configuration = configuration;

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
            try
            {
                if (stripeEvent.Type == Events.PaymentIntentSucceeded)
                {
                    var intent = stripeEvent.Data.Object as PaymentIntent;
                    if (intent == null) return;

                    var paymentMessage = new PaymentProcessingMessage { PaymentIntentId = intent.Id };
                    var messageBody = JsonConvert.SerializeObject(paymentMessage);

                    await _rabbitMQService.PublishAsync("payment_queue", messageBody);
                }
            }
            catch(Exception ex)
            {

            }
        }

        #endregion
    }
}
