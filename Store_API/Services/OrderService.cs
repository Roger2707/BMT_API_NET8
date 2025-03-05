using Microsoft.EntityFrameworkCore;
using Store_API.Data;
using Store_API.DTOs.Baskets;
using Store_API.DTOs.Orders;
using Store_API.Helpers;
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
        public OrderService(IUnitOfWork unitOfWork, IPaymentService paymentService)
        {
            _unitOfWork = unitOfWork;
            _paymentService = paymentService;
        }

        public async Task<OrderResponseDTO> Create(int userId, BasketDTO basket, int userAddressId, UserAddressDTO userAddressDTO)
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
                                ProductId = i.ProductId,
                                Quantity = i.Quantity,
                                SubTotal = i.DiscountPrice * i.Quantity,
                            })
                    .ToList();

                // 2. Calc Grand Total
                double grandTotal = orderItems.Sum(item => item.SubTotal);

                // 3. Handle User Address
                int addressId = userAddressId;
                if (userAddressId == 0)
                {
                    addressId = (await _unitOfWork.UserAddress.UpsertUserAddresses(userId, userAddressDTO)).Data;
                }

                // 3. Add Order and Remove/Clear Items in Basket
                var order = new Order
                {
                    UserId = userId,
                    OrderDate = DateTime.Now,
                    Status = OrderStatus.Pending,
                    UserAddressId = addressId,
                    Items = orderItems,
                    GrandTotal = grandTotal,
                    DeliveryFee = grandTotal > 100 ? 0 : 10,
                };

                await _unitOfWork.Order.Create(order);
                await _unitOfWork.SaveChangesAsync();

                // 4. Remove Items in Basket
                var items = basket.Items.Where(x => x.Status == true).ToList();
                await _unitOfWork.Basket.RemoveRange(items);

                // 5. Create PaymentIntent on Stripe
                var paymentIntent = await _paymentService.CreatePaymentIntentAsync(order.Id, grandTotal);

                // 6. Create Payment in DB
                var payment = new Payment 
                { 
                    OrderId = order.Id
                    , PaymentIntentId = paymentIntent.Id
                    , Amount = grandTotal
                    , Status = OrderStatus.Pending, CreatedAt = DateTime.UtcNow 
                };
                await _paymentService.AddAsync(payment);

                // 7. Save and Commit
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
    }
}
