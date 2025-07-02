using MassTransit;
using Newtonsoft.Json;
using Store_API.Contracts;
using Store_API.DTOs.Baskets;
using Store_API.DTOs.Orders;
using Store_API.DTOs.Stocks;
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
        private readonly PaymentIntentService _paymentIntentService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        private readonly IBasketService _basketService;
        private readonly IOrderService _orderService;
        private readonly IStockService _stockService;

        private readonly EmailSenderService _emailSenderService;
        private readonly IPublishEndpoint _publishEndpoint;

        public PaymentService
        (   PaymentIntentService paymentIntentService, IUnitOfWork unitOfWork, IUserService userService
            , IBasketService basketService, IOrderService orderService, IStockService stockService
            , EmailSenderService emailSenderService, IPublishEndpoint publishEndpoint
        )
        {
            _paymentIntentService = paymentIntentService;
            _unitOfWork = unitOfWork;
            _userService = userService;
            _basketService = basketService;
            _orderService = orderService;
            _stockService = stockService;
            _emailSenderService = emailSenderService;
            _publishEndpoint = publishEndpoint;
        }

        #region CRUD PaymentIntent

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

        public async Task CreatePaymentAsync(int userId, string username, ShippingAddressDTO shippingAddress)
        {
            var basket = await _basketService.GetBasketDTO(userId, username);
            var requestId = Guid.NewGuid();
            if (basket == null) throw new Exception($"Basket not found for UserId: {userId}");
            var payment = new Payment
            {
                Id = requestId,
                UserId = userId,
                PaymentIntentId = "",
                ClientSecret = "",
                Amount = 0,
                BasketHash = "",
                Status = PaymentStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                OrderId = Guid.Empty
            };
            await _unitOfWork.Payment.AddAsync(payment);
            await _publishEndpoint.Publish(new PaymentIntentCreated(requestId, userId, basket.Items, shippingAddress));
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<PaymentIntent> CreatePaymentIntentAsync(int userId, Guid requestId, List<BasketItemDTO> items, ShippingAddressDTO shippingAddress)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var itemsSelected = items.Where(item => item.Status == true);
                // 1. Check Quantity Inventory -> Export Stock if available
                foreach (var item in itemsSelected)
                {
                    var stockAvailable = await _unitOfWork.Stock.GetAvailableStock(item.ProductDetailId, item.Quantity);
                    if (stockAvailable == null)
                        throw new Exception($"Sorry! Product {item.ProductName} is not enoungh quantity in Inventory !");

                    await _stockService.ExportStock(new StockUpsertDTO
                    {
                        ProductDetailId = item.ProductDetailId,
                        WarehouseId = stockAvailable.WarehouseId,
                        Quantity = item.Quantity,
                        StockId = stockAvailable.StockId
                    });
                }

                // 2. Create BasketHash
                string basketHash = GenerateCartHash(itemsSelected);

                // 3. Check existed PaymentIntent -> Get latest pending payment for this user
                var existingPayment = await _unitOfWork.Payment.GetLatestPendingPaymentAsync(userId);
                bool isNewPayment = existingPayment == null || existingPayment.BasketHash != basketHash;
                if (isNewPayment == false)
                {
                    var existingStripeIntent = await _paymentIntentService.GetAsync(existingPayment.PaymentIntentId);
                    if (existingStripeIntent.Status is "requires_payment_method" or "requires_confirmation"
                        or "requires_action" or "processing")
                    {
                        await _unitOfWork.CommitAsync();
                        return existingStripeIntent;
                    }
                }

                // 4. Handle money rate (vnd -> usd)
                decimal exchangeRate = 0.000039m;
                double grandTotal = itemsSelected.Sum(item => item.Quantity * item.DiscountPrice);
                long amountInCents = (long)(CF.GetDecimal(grandTotal) * exchangeRate * 100);

                // 5. Create PaymentIntent
                var options = new PaymentIntentCreateOptions
                {
                    Amount = amountInCents,
                    Currency = "usd",

                    // ## 3D Secure - use for SCA (Strong Customer Authentication)
                    AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions { Enabled = true },

                    // Shipping Adress
                    Metadata = new Dictionary<string, string> { { "ShippingAdress", JsonConvert.SerializeObject(shippingAddress) }, }
                };
                var paymentIntent = await _paymentIntentService.CreateAsync(options);

                // 6. Update Payments
                var payment = await _unitOfWork.Payment.FindFirstAsync(x => x.Id == requestId);
                payment.PaymentIntentId = paymentIntent.Id;
                payment.ClientSecret = paymentIntent.ClientSecret;
                payment.Amount = grandTotal;
                payment.BasketHash = basketHash;

                // 7. Publish StockHoldCreated Event: *event must be publish before the last save changes
                await _publishEndpoint.Publish(new StockHoldCreated(paymentIntent.Id, userId, itemsSelected.ToList()));

                // 8. Save changes and Commit transaction
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

            // 2. Idempotency check
            var payment = await _unitOfWork.Payment.FindFirstAsync(x => x.PaymentIntentId == paymentIntent.Id);
            if (payment == null)
                throw new InvalidOperationException($"Payment not found for IntentId: {paymentIntent.Id}");

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

            await _emailSenderService.SendEmailAsync(user.Email, "[ 🔥🔥🔥 ] ORDER SUCCESS:", content);
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
