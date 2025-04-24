using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Store_API.DTOs.Baskets;
using Store_API.DTOs.Orders;
using Store_API.DTOs.Payments;
using Store_API.DTOs.Stocks;
using Store_API.Enums;
using Store_API.Helpers;
using Store_API.IService;
using Store_API.Models;
using Store_API.Repositories;
using Store_API.SignalIR;
using Stripe;

namespace Store_API.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        private readonly IStockService _stockService;
        private readonly IBasketService _basketService;
        private readonly IOrderService _orderService;

        private readonly string _apiKey;
        private readonly IConfiguration _configuration;
        private readonly EmailSenderService _emailSenderService;

        private readonly IHubContext<OrderStatusHub> _hubContext;

        public PaymentService
        (   IUnitOfWork unitOfWork, IStockService stockService
            , IConfiguration configuration, EmailSenderService emailSenderService
            , IHubContext<OrderStatusHub> hubContext, IUserService userService, IBasketService basketService
            , IOrderService orderService
        )
        {
            _unitOfWork = unitOfWork;
            _stockService = stockService;
            _configuration = configuration;
            _emailSenderService = emailSenderService;
            _hubContext = hubContext;
            _userService = userService;
            _basketService = basketService;
            _orderService = orderService;

            _apiKey = _configuration["Stripe:SecretKey"];

            if (string.IsNullOrEmpty(_apiKey)) throw new Exception("Stripe API Key is missing from configuration.");
            if (string.IsNullOrEmpty(StripeConfiguration.ApiKey))
                StripeConfiguration.ApiKey = _apiKey;
        }

        #region Payment Methods

        public async Task<PaymentIntent> CreatePaymentIntentAsync(BasketDTO basket, int userAddressId)
        {
            await _unitOfWork.BeginTransactionAsync(TransactionType.Both);
            try
            {
                // 1. Check Quantity Product in Inventory
                foreach (var item in basket.Items)
                {
                    var stockExist = await _unitOfWork.Stock.GetStockExisted(item.ProductDetailId);
                    if (stockExist == null || stockExist.Quantity < item.Quantity)
                        throw new Exception($"Sorry! Product {item.ProductName} is not enoungh quantity in Inventory !");
                }

                // 2. Calc Grand Total
                double grandTotal = basket.Items.Sum(item => item.Quantity * item.DiscountPrice);

                // 3. Exchange rate (vnd -> usd)
                decimal exchangeRate = 0.000039m;
                long amountInCents = (long)(CF.GetDecimal(grandTotal) * exchangeRate * 100);

                // 4. Create Payment Intent Options
                var options = new PaymentIntentCreateOptions
                {
                    Amount = amountInCents,
                    Currency = "usd",
                    //Metadata = new Dictionary<string, string>
                    //{
                    //    { "orderId", orderId.ToString() } 
                    //},

                    // ## 3D Secure - use for SCA (Strong Customer Authentication)
                    AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                    {
                        Enabled = true
                    }
                };

                // 5. Create PaymentIntent
                var service = new PaymentIntentService();
                var paymentIntent = await service.CreateAsync(options);

                // 6. Create Payment in DB
                var payment = new Payment
                {
                    UserId = basket.UserId,
                    UserAddressId = userAddressId,
                    PaymentIntentId = paymentIntent.Id,
                    Amount = grandTotal,
                    Status = PaymentStatus.Pending,
                    CreatedAt = DateTime.UtcNow,
                };
                await _unitOfWork.Payment.AddAsync(payment);

                // 7. SaveChanges and Commit 
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();

                return paymentIntent;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }           
        }

        public async Task HandleStripeWebhookAsync(Event stripeEvent)
        {
            await _unitOfWork.BeginTransactionAsync(TransactionType.Both);
            try
            {
                if (stripeEvent.Type == Events.PaymentIntentSucceeded)
                {
                    await HandlePaymentIntentSuccess(stripeEvent);
                }
                else if(stripeEvent.Type == Events.PaymentIntentPaymentFailed)
                {
                    await HandlePaymentIntentFailed(stripeEvent);
                }

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();
            }
            catch(Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        #endregion

        #region Stripe Payment Intent Events

        private async Task HandlePaymentIntentSuccess(Event stripeEvent)
        {
            var orderId = Guid.NewGuid();
            // 1. Handle Stripe webhook event
            var intent = stripeEvent.Data.Object as PaymentIntent;
            if (intent == null) return;

            var paymentMessage = new PaymentProcessingMessage { PaymentIntentId = intent.Id };
            var messageBody = JsonConvert.SerializeObject(paymentMessage);

            // 2. Idempotency check
            var payment = await _unitOfWork.Payment.GetByPaymentIntent(paymentMessage.PaymentIntentId);
            if (payment == null)
                throw new InvalidOperationException($"Payment not found for IntentId: {paymentMessage.PaymentIntentId}");

            if (payment.Status == PaymentStatus.Succeeded)
                return;

            // 3. Find User / Basket
            var user = await _userService.GetUser(payment.UserId);
            var basket = await _unitOfWork.Basket.GetBasketDTORedis(payment.UserId, user.UserName);

            // 4. Check Inventory and Update StockTransaction (Export)
            string orderItemText = "";
            foreach (var item in basket.Items)
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

            // 5. Create Order 
            var orderCreateRequest = new OrderCreateRequest
            {
                OrderId = orderId,
                UserId = payment.UserId,
                Username = user.UserName,
                Email = user.Email,
                UserAddressId = payment.UserAddressId,
                BasketDTO = basket,
                Amount = payment.Amount,
                ClientSecret = intent.ClientSecret,
            };
            await _orderService.Create(orderCreateRequest);

            // 6. Clear Basket - Sync Redis
            var items = basket.Items.Where(x => x.Status == true).ToList();
            await _basketService.RemoveRangeItems(user.UserName, payment.UserId, basket.Id);

            // 5. Update Status Order and Payment -> Check out 
            payment.Status = PaymentStatus.Succeeded;

            // 6. Send Email
            string content = $@"    
                                    Thank you for shopping at ROGER STORE
                                    Your ORDER info: {orderId}

                                    {orderItemText}

                                    Hope you 'll have a good day. Thank you!
                                                                [ROGER's Customer Service Team]                                       
                                ";

            await _emailSenderService.SendEmailAsync(user.Email, "[NEW] 🔥 Your Check out:", content);

            // 7. Signal IR - Send Message To Client
            var orderUpdateMessage = new OrderUpdateHub
            {
                OrderId = orderId,
                OrderStatus = OrderStatus.Shipping,
            };
            // All Admins is Received
            await _hubContext.Clients.Group("Admins").SendAsync("ReceiveOrderUpdate", orderUpdateMessage);
            // User (Order of login user) is Received
            await _hubContext.Clients.User(user.Id.ToString()).SendAsync("ReceiveOrderUpdate", orderUpdateMessage);
        }

        private async Task HandlePaymentIntentFailed(Event stripeEvent)
        {
            var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
            if (paymentIntent == null) return;

            var payment = await _unitOfWork.Payment.GetByPaymentIntent(paymentIntent.Id);
            payment.Status = PaymentStatus.Failed;
        }

        #endregion
    }
}
