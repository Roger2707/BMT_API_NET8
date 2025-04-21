using Store_API.DTOs.Baskets;
using Store_API.DTOs.Orders;
using Store_API.Enums;
using Store_API.IService;
using Store_API.Models.OrderAggregate;
using Store_API.Repositories;

namespace Store_API.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentService _paymentService;
        private readonly IBasketService _basketService;
        public OrderService(IUnitOfWork unitOfWork, IPaymentService paymentService, IBasketService basketService)
        {
            _unitOfWork = unitOfWork;
            _paymentService = paymentService;
            _basketService = basketService;
        }

        #region Create Order

        public async Task<OrderResponseDTO> Create(int userId, string userName, string email, BasketDTO basket, int userAddressId)
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

                // 2. Get Order Items
                var orderItems = basket.Items
                    .Where(item => item.Status)
                    .Select(i => 
                            new OrderItem
                            {
                                ProductDetailId = i.ProductDetailId,
                                Quantity = i.Quantity,
                                SubTotal = i.DiscountPrice * i.Quantity,
                            })
                    .ToList();

                // 3. Calc Grand Total
                double grandTotal = orderItems.Sum(item => item.SubTotal);

                // 4. Add Order in DB
                var order = new Order
                {
                    UserId = userId,
                    Email = email,
                    OrderDate = DateTime.Now,
                    Status = OrderStatus.Pending,
                    UserAddressId = userAddressId,
                    Items = orderItems,
                    GrandTotal = grandTotal,
                    DeliveryFee = grandTotal > 1000000 ? 0 : 50000,
                };

                // SaveChange in order to get Id
                await _unitOfWork.Order.Create(order);
                await _unitOfWork.SaveChangesAsync();

                // 5. Remove Items in Basket - Sync Redis
                var items = basket.Items.Where(x => x.Status == true).ToList();
                await _basketService.RemoveRangeItems(userName, userId, basket.Id);

                // 6. Create PaymentIntent on Stripe (Add Payment in db)
                var paymentIntent = await _paymentService.CreatePaymentIntentAsync(order.Id, grandTotal);
                order.ClientSecret = paymentIntent.ClientSecret;

                // 7. Save and Commit -> update database
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();

                return new OrderResponseDTO
                {
                    Id = order.Id,
                    GrandTotal = order.GrandTotal,
                    CreatedAt = DateTime.UtcNow,
                    OrderStatus = OrderStatus.Pending,
                    ClientSecret = paymentIntent.ClientSecret
                };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        #endregion

        #region Retrieve 

        public async Task<OrderDTO> GetOrder(int orderId)
        {
            var orderDTO = await _unitOfWork.Order.GetOrder(orderId);
            return orderDTO;
        }

        public async Task<IEnumerable<OrderItemDapperRow>> GetOrder(string clientSecret)
        {
            var orderDTO = await _unitOfWork.Order.GetOrder(clientSecret);
            return orderDTO;
        }

        public async Task<IEnumerable<OrderDTO>> GetOrders(int orderId)
        {
            var orders = await _unitOfWork.Order.GetOrders(orderId);
            return orders;
        }

        #endregion

        #region Helpers

        public async Task UpdateOrderStatus(int orderId, OrderStatus status)
        {
            await _unitOfWork.Order.UpdateOrderStatus(orderId, status);
            await _unitOfWork.SaveChangesAsync();
        }

        #endregion 
    }
}
