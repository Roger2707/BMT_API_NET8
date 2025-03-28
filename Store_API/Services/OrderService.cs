using Store_API.DTOs.Baskets;
using Store_API.DTOs.Orders;
using Store_API.IService;
using Store_API.Models;
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

        public async Task<OrderResponseDTO> Create(int userId, string userName, BasketDTO basket, int userAddressId)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                // 1. Get Order Items
                var orderItems = basket.Items
                    .Where(item => item.Status)
                    .Select(i => 
                            new OrderItem
                            {
                                ProductId = i.ProducDetailtId,
                                Quantity = i.Quantity,
                                SubTotal = i.DiscountPrice * i.Quantity,
                            })
                    .ToList();

                // 2. Calc Grand Total
                double grandTotal = orderItems.Sum(item => item.SubTotal);

                // 3. Add Order in DB
                var order = new Order
                {
                    UserId = userId,
                    OrderDate = DateTime.Now,
                    Status = OrderStatus.Pending,
                    UserAddressId = userAddressId,
                    Items = orderItems,
                    GrandTotal = grandTotal,
                    DeliveryFee = grandTotal > 100 ? 0 : 10,
                };

                await _unitOfWork.Order.Create(order);
                await _unitOfWork.SaveChangesAsync();

                // 4. Remove Items in Basket - Sync Redis
                var items = basket.Items.Where(x => x.Status == true).ToList();
                await _basketService.RemoveRange(userName, items);

                // 5. Create PaymentIntent on Stripe (Add Payment in db)
                var paymentIntent = await _paymentService.CreatePaymentIntentAsync(order.Id, grandTotal);

                // 6. Save and Commit
                await _unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();

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
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<OrderDTO> GetOrder(int orderId)
        {
            var orderDTO = await _unitOfWork.Order.GetOrder(orderId);
            return orderDTO;
        }

        public async Task<IEnumerable<OrderDTO>> GetOrders(int orderId)
        {
            var orders = await _unitOfWork.Order.GetOrders(orderId);
            return orders;
        }

        #region Helpers

        public async Task UpdateOrderStatus(int orderId, OrderStatus status)
        {
            await _unitOfWork.Order.UpdateOrderStatus(orderId, status);
            await _unitOfWork.SaveChangesAsync();
        }

        #endregion 
    }
}
