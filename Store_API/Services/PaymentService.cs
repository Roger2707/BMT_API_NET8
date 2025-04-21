using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Store_API.DTOs.Orders;
using Store_API.DTOs.Payments;
using Store_API.DTOs.Stocks;
using Store_API.Enums;
using Store_API.Helpers;
using Store_API.IService;
using Store_API.Models;
using Store_API.Models.OrderAggregate;
using Store_API.Repositories;
using Store_API.SignalIR;
using Stripe;

namespace Store_API.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStockService _stockService;
        private readonly IConfiguration _configuration;
        private readonly EmailSenderService _emailSenderService;
        private readonly IHubContext<OrderStatusHub> _hubContext;
        private readonly string _apiKey;

        public PaymentService
        (   IUnitOfWork unitOfWork, IStockService stockService
            , IConfiguration configuration, EmailSenderService emailSenderService
            , IHubContext<OrderStatusHub> hubContext
        )
        {
            _unitOfWork = unitOfWork;
            _stockService = stockService;
            _configuration = configuration;
            _emailSenderService = emailSenderService;
            _hubContext = hubContext;

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
                Metadata = new Dictionary<string, string>
                {
                    { "orderId", orderId.ToString() }  // use for signal IR
                },

                // ## 3D Secure
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true
                }
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
                        throw new InvalidOperationException($"Payment not found for IntentId: {paymentMessage.PaymentIntentId}");

                    // 2. ✅ Idempotency check
                    if (payment.Status == PaymentStatus.Succeeded)
                        return;

                    // 3. Find related order
                    var order = await _unitOfWork.Order.FirstOrDefaultAsync(payment.OrderId);
                    if (order == null)
                        throw new InvalidOperationException($"Order not found for PaymentIntentId: {paymentMessage.PaymentIntentId}");

                    // 4. Check Inventory and Update StockTransaction (Export)
                    string orderItemText = "";
                    foreach (var item in order.Items)
                    {
                        var stockExist = await _unitOfWork.Stock.GetStockExisted(item.ProductDetailId);
                        if (stockExist == null || stockExist.Quantity < item.Quantity)
                            throw new InvalidOperationException($"Sorry! Product is not enoungh quantity in Inventory !");

                        var stockUpsertDTO = new StockUpsertDTO
                        {
                            ProductDetailId = item.ProductDetailId,
                            WarehouseId = stockExist.WarehouseId,
                            Quantity = item.Quantity,
                        };
                        await _stockService.ExportStock(stockUpsertDTO);

                        // 4.1 Add to order item text for email
                        orderItemText += $"{item.ProductDetailId} - {item.Quantity} pcs\n";
                    }

                    // 5. Update Status Order and Payment -> Check out 
                    payment.Status = PaymentStatus.Succeeded;
                    order.Status = OrderStatus.Shipping;

                    // 6. Send Email
                    string content = $@"    
                                    Thank you for shopping at ROGER STORE
                                    Your ORDER info: {order.Id}

                                    {orderItemText}

                                    Hope you 'll have a good day. Thank you!
                                                                [ROGER's Customer Service Team]                                       
                                ";

                    await _emailSenderService.SendEmailAsync(order.Email, "[NEW] 🔥 Your Check out:", content);

                    // 7. Signal IR - Send Message To Client
                    var orderUpdateMessage = new OrderUpdateHub
                    {
                        Id = order.Id,
                        OrderStatus = OrderStatus.Shipping,
                    };
                    // All Admins is Received
                    await _hubContext.Clients.Group("Admins").SendAsync("ReceiveOrderUpdate", orderUpdateMessage);
                    // User (Order of login user) is Received
                    await _hubContext.Clients.User(order.UserId.ToString()).SendAsync("ReceiveOrderUpdate", orderUpdateMessage);

                    // 8. Save DB
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
