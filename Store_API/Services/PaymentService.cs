using Newtonsoft.Json;
using Store_API.DTOs.Payments;
using Store_API.DTOs.Stocks;
using Store_API.Enums;
using Store_API.Helpers;
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
        private readonly IStockService _stockService;
        private readonly IConfiguration _configuration;
        private readonly string _apiKey;

        public PaymentService(IUnitOfWork unitOfWork, IStockService stockService, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _stockService = stockService;
            _configuration = configuration;
            _apiKey = _configuration["Stripe:SecretKey"];

            if (string.IsNullOrEmpty(_apiKey)) throw new Exception("Stripe API Key is missing from configuration.");
            if (string.IsNullOrEmpty(StripeConfiguration.ApiKey))
                StripeConfiguration.ApiKey = _apiKey;
        }

        #region Payment Methods

        public async Task<PaymentIntent> CreatePaymentIntentAsync(int orderId, double amount)
        {
            // 1. Exchange rate (vnd -> usd)
            decimal exchangeRate = 0.000039m;
            long amountInCents = (long)(CF.GetDecimal(amount) * exchangeRate * 100);
            var options = new PaymentIntentCreateOptions
            {
                Amount = amountInCents,
                Currency = "usd",
                PaymentMethodTypes = new List<string> { "card" }
            };

            // 2. Create PaymentIntent
            var service = new PaymentIntentService();
            var paymentIntent = await service.CreateAsync(options);

            var payment = new Payment
            {
                OrderId = orderId,
                PaymentIntentId = paymentIntent.Id,
                Amount = amount,
                Status = PaymentStatus.Pending,
                CreatedAt = DateTime.UtcNow,
            };
            await _unitOfWork.Payment.AddAsync(payment);
            return paymentIntent;
        }

        public async Task HandleStripeWebhookAsync(Event stripeEvent)
        {
            await _unitOfWork.BeginTransactionAsync(TransactionType.Both);
            try
            {
                if (stripeEvent.Type == Events.PaymentIntentSucceeded)
                {
                    // 1. Handle Stripe webhook event
                    var intent = stripeEvent.Data.Object as PaymentIntent;
                    if (intent == null) return;

                    var paymentMessage = new PaymentProcessingMessage { PaymentIntentId = intent.Id };
                    var messageBody = JsonConvert.SerializeObject(paymentMessage);

                    var payment = await _unitOfWork.Payment.GetByPaymentIntent(paymentMessage.PaymentIntentId);
                    if (payment == null)
                        throw new Exception($"Payment not found for IntentId: {paymentMessage.PaymentIntentId}");

                    // 2. Find related order
                    var order = await _unitOfWork.Order.FirstOrDefaultAsync(payment.OrderId);
                    if (order == null)
                        throw new Exception($"Order not found for PaymentIntentId: {paymentMessage.PaymentIntentId}");

                    // 3. Check Inventory and Update StockTransaction (Export)
                    foreach (var item in order.Items)
                    {
                        var stockExist = await _unitOfWork.Stock.GetStockExisted(item.ProductDetailId);
                        if (stockExist == null || stockExist.Quantity < item.Quantity)
                            throw new Exception($"Sorry! Product is not enoungh quantity in Inventory !");

                        var stockUpsertDTO = new StockUpsertDTO
                        {
                            ProductDetailId = item.ProductDetailId,
                            WarehouseId = stockExist.WarehouseId,
                            Quantity = item.Quantity,
                        };
                        await _stockService.ExportStock(stockUpsertDTO);
                    }

                    // 4. Update Status Order and Payment -> Check out 
                    payment.Status = PaymentStatus.Succeeded;
                    order.Status = OrderStatus.Shipping;

                    // 5. Save DB
                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.CommitAsync();
                }
                else if(stripeEvent.Type == Events.PaymentIntentCanceled)
                {

                }
            }
            catch(Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        #endregion
    }
}
