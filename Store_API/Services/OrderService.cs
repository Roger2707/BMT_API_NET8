using Microsoft.EntityFrameworkCore;
using Store_API.Data;
using Store_API.DTOs.Baskets;
using Store_API.DTOs.Orders;
using Store_API.Helpers;
using Store_API.IService;
using Store_API.Models.OrderAggregate;
using Store_API.Repositories;

namespace Store_API.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        public OrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<int> Create(int userId, BasketDTO basket, int userAddressId, UserAddressDTO userAddressDTO)
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

                // 4. Remove Items in Basket
                var items = basket.Items.Where(x => x.Status == true).ToList();
                await _unitOfWork.Basket.RemoveRange(items);

                // 5. Save 
                await _unitOfWork.SaveChangesAsync();

                await transaction.CommitAsync();
                return order.Id;
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
