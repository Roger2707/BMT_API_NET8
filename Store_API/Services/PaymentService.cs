using MassTransit;
using Newtonsoft.Json;
using Store_API.Contracts;
using Store_API.DTOs.Baskets;
using Store_API.DTOs.Orders;
using Store_API.DTOs.Payments;
using Store_API.Enums;
using Store_API.Helpers;
using Store_API.Infrastructures;
using Store_API.Models;
using Store_API.Models.OrderAggregate;
using Store_API.Services.IService;
using Stripe;
using System.Security.Cryptography;
using System.Text;

namespace Store_API.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        private readonly IBasketService _basketService;
        private readonly IOrderService _orderService;

        private readonly string _apiKey;
        private readonly IConfiguration _configuration;
        private readonly EmailSenderService _emailSenderService;
        private readonly IPublishEndpoint _publishEndpoint;

        public PaymentService
        (   IUnitOfWork unitOfWork, IConfiguration configuration, EmailSenderService emailSenderService
            , IUserService userService, IBasketService basketService, IOrderService orderService, IPublishEndpoint publishEndpoint
        )
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _emailSenderService = emailSenderService;
            _userService = userService;
            _basketService = basketService;
            _orderService = orderService;
            _publishEndpoint = publishEndpoint;
            _apiKey = _configuration["Stripe:SecretKey"];

            if (string.IsNullOrEmpty(_apiKey)) throw new Exception("Stripe API Key is missing from configuration.");
            if (string.IsNullOrEmpty(StripeConfiguration.ApiKey))
                StripeConfiguration.ApiKey = _apiKey;
        }

        #region CRUD PaymentItent

        public string GenerateCartHash(IEnumerable<BasketItemDTO> cartItems)
        {
            // 1. Sort
            var sortedItems = cartItems.OrderBy(i => i.ProductDetailId);
            // 2. Concat string
            var rawString = string.Join(";", sortedItems.Select(i => $"{i.ProductDetailId}-{i.Quantity}-{i.DiscountPrice:F2}"));
            // 3. Generate SHA256 hash
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(rawString);
            var hashBytes = sha256.ComputeHash(bytes);
            //4 Convert to hex string
            var hash = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
            return hash;
        }

        public async Task<PaymentIntent> CreatePaymentIntentAsync(BasketDTO basket, ShippingAddressDTO shippingAddress)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                // 1. Check Quantity Product in Inventory
                foreach (var item in basket.Items)
                {
                    var stockExist = await _unitOfWork.Stock.GetStockExisted(item.ProductDetailId);
                    if (stockExist == null || stockExist.Quantity < item.Quantity)
                        throw new Exception($"Sorry! Product {item.ProductName} is not enoungh quantity in Inventory !");
                }
                var stripeService = new PaymentIntentService();

                // 2. Calc Grand Total
                double grandTotal = basket.Items.Sum(item => item.Quantity * item.DiscountPrice);

                // 3. Create BasketHash
                string basketHash = GenerateCartHash(basket.Items);

                // 4. Check existed PaymentIntent
                var existingPayment = await _unitOfWork.Payment.GetLatestPendingPaymentAsync(basket.UserId);
                if (existingPayment != null)
                {
                    var existingStripeIntent = await stripeService.GetAsync(existingPayment.PaymentIntentId);

                    if (existingPayment.BasketHash == basketHash && existingStripeIntent.Status == "requires_payment_method")
                    {
                        await _unitOfWork.CommitAsync();
                        return existingStripeIntent;
                    }

                    if (existingStripeIntent.Status == "succeeded")
                    {
                        await _unitOfWork.CommitAsync();
                        throw new Exception("An existing payment was already completed for a different cart.");
                    }

                    if (existingStripeIntent.Status == "requires_payment_method" || existingStripeIntent.Status == "requires_confirmation")
                    {
                        try
                        {
                            await stripeService.CancelAsync(existingPayment.PaymentIntentId);
                        }
                        catch { }
                        _unitOfWork.Payment.DeleteAsync(existingPayment);
                    }
                }

                // 5. Exchange rate (vnd -> usd)
                decimal exchangeRate = 0.000039m;
                long amountInCents = (long)(CF.GetDecimal(grandTotal) * exchangeRate * 100);

                // 6. Create Payment Intent Options
                var options = new PaymentIntentCreateOptions
                {
                    Amount = amountInCents,
                    Currency = "usd",

                    // ## 3D Secure - use for SCA (Strong Customer Authentication)
                    AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                    {
                        Enabled = true
                    },

                    // Shipping Adress
                    Metadata = new Dictionary<string, string>
                    {
                        { "ShippingAdress", JsonConvert.SerializeObject(shippingAddress) },
                    }
                };

                // 7. Create PaymentIntent
                var paymentIntent = await stripeService.CreateAsync(options);

                // 8. Create Payment in DB
                var payment = new Payment
                {
                    UserId = basket.UserId,
                    PaymentIntentId = paymentIntent.Id,
                    ClientSecret = paymentIntent.ClientSecret,
                    Amount = grandTotal,
                    BasketHash = basketHash,
                    Status = PaymentStatus.Pending,
                    CreatedAt = DateTime.UtcNow,
                    OrderId = Guid.Empty
                };
                await _unitOfWork.Payment.AddAsync(payment);

                // 9. Publish StockHoldCreated Event
                await _publishEndpoint.Publish(new StockHoldCreated(paymentIntent.Id, basket.UserId, basket.Items));

                // 10. SaveChanges and Commit 
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

        #endregion

        #region Stripe Payment Intent Events

        public async Task HandleStripeWebhookAsync(Stripe.Event stripeEvent)
        {
            await _unitOfWork.BeginTransactionAsync(TransactionType.Both);
            try
            {
                if (stripeEvent.Type == Events.PaymentIntentSucceeded)
                {
                    await HandlePaymentIntentSuccess(stripeEvent);
                }
                else if (stripeEvent.Type == Events.PaymentIntentPaymentFailed)
                {
                    await HandlePaymentIntentFailed(stripeEvent);
                }

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        private async Task HandlePaymentIntentSuccess(Stripe.Event stripeEvent)
        {
            var orderId = Guid.NewGuid();
            // 1. Handle Stripe webhook event
            var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
            if (paymentIntent == null) return;

            var paymentMessage = new PaymentProcessingMessage { PaymentIntentId = paymentIntent.Id };
            var messageBody = JsonConvert.SerializeObject(paymentMessage);

            // 2. Idempotency check
            var payment = await _unitOfWork.Payment.FindFirstAsync(x => x.PaymentIntentId == paymentMessage.PaymentIntentId);
            if (payment == null)
                throw new InvalidOperationException($"Payment not found for IntentId: {paymentMessage.PaymentIntentId}");

            if (payment.Status == PaymentStatus.Success)
                return;

            // 3. Find User / Basket
            var user = await _userService.GetUser(payment.UserId);
            var basket = await _unitOfWork.Basket.GetBasketDTORedis(payment.UserId, user.UserName);

            // 4. Get stock hold items -> Update status to Confirmed
            var stockHold = await _unitOfWork.StockHold.FindFirstAsync(x => x.PaymentIntentId == paymentIntent.Id);
            if (stockHold == null)
                throw new InvalidOperationException($"Stock hold not found for PaymentIntentId: {paymentIntent.Id}");

            if (stockHold.Status == StockHoldStatus.Confirmed)
                throw new InvalidOperationException($"Stock hold already confirmed for PaymentIntentId: {paymentIntent.Id}");

            stockHold.Status = StockHoldStatus.Confirmed;

            // 5. Create Order
            var shippingAddressDTO = JsonConvert.DeserializeObject<ShippingAddressDTO>(paymentIntent.Metadata["ShippingAdress"]);
            var shippingAddress = new ShippingAddress
            {
                City = shippingAddressDTO.City,
                District = shippingAddressDTO.District,
                Ward = shippingAddressDTO.Ward,
                StreetAddress = shippingAddressDTO.StreetAddress,
                PostalCode = shippingAddressDTO.PostalCode,
                Country = shippingAddressDTO.Country,
            };
            var orderCreateRequest = new OrderCreateRequest
            {
                OrderId = orderId,
                UserId = payment.UserId,
                Username = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                ShippingAdress = shippingAddress,
                BasketDTO = basket,
                Amount = payment.Amount,
                ClientSecret = paymentIntent.ClientSecret,
            };
            await _orderService.Create(orderCreateRequest);

            // 6. Clear Basket - Sync Redis
            var items = basket.Items.Where(x => x.Status == true).ToList();
            await _basketService.RemoveRangeItems(user.UserName, payment.UserId, basket.Id);

            // 7. Check Save Address
            var isSaveAddress = shippingAddressDTO.IsSaveAddress;
            if(isSaveAddress)
            {
                await _unitOfWork.UserAddress.AddAsync(new Models.Users.UserAddress
                {
                    UserId = payment.UserId,
                    ShippingAddress = shippingAddress,
                });
            }

            // 8. Update Status Payment, Add OrderId
            payment.Status = PaymentStatus.Success;
            payment.OrderId = orderId;

            // 9. Send Email
            var orderItemText = string.Join("\n", 
                basket.Items.Select(item => $"{item.ProductDetailId} - {item.Quantity} pcs"));

            string content = $@" 
                                Thank you for shopping at ROGER STORE
                                Your ORDER ID: {orderId}

                                {orderItemText}

                                Hope you 'll have a good day. Thank you!
                                                            [ROGER BMT's Customer Service Team]                                       
                                ";

            await _emailSenderService.SendEmailAsync(user.Email, "[🔥🔥🔥] ORDER SUCCESS:", content);
        }

        private async Task HandlePaymentIntentFailed(Stripe.Event stripeEvent)
        {
            var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
            if (paymentIntent == null) return;

            var payment = await _unitOfWork.Payment.FindFirstAsync(x => x.PaymentIntentId == paymentIntent.Id);
            payment.Status = PaymentStatus.Failed;
        }

        #endregion
    }
}
