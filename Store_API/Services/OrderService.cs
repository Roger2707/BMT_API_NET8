using Microsoft.AspNetCore.SignalR;
using Store_API.DTOs.Orders;
using Store_API.Enums;
using Store_API.IService;
using Store_API.Models.OrderAggregate;
using Store_API.Models.Users;
using Store_API.Repositories;
using Store_API.SignalR;

namespace Store_API.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHubContext<OrdersHub> _hubContext;
        public OrderService(IUnitOfWork unitOfWork, IHubContext<OrdersHub> hubContext)
        {
            _unitOfWork = unitOfWork;
            _hubContext = hubContext;
        }

        #region Create Order

        public async Task Create(OrderCreateRequest orderCreateRequest)
        {
            // 1. Get Order Items
            var orderItems = orderCreateRequest.BasketDTO.Items
                .Where(item => item.Status)
                .Select(i =>
                        new OrderItem
                        {
                            ProductOrderItem = new ProductOrderItem
                            {
                                ProductDetailId = i.ProductDetailId,
                                ProductName = i.ProductName,
                                ProductImageUrl = i.ProductFirstImage,
                                ProductPrice = i.DiscountPrice,
                            },
                            Quantity = i.Quantity,
                            SubTotal = i.DiscountPrice * i.Quantity,
                        })
                .ToList();

            // 2. Add Order in DB
            var order = new Order
            {
                Id = orderCreateRequest.OrderId,
                UserId = orderCreateRequest.UserId,
                Email = orderCreateRequest.Email,
                OrderDate = DateTime.Now,
                Status = OrderStatus.Created,
                ShippingAddress = orderCreateRequest.ShippingAdress,
                Items = orderItems,
                GrandTotal = orderCreateRequest.Amount,
                DeliveryFee = orderCreateRequest.Amount > 1000000 ? 0 : 50000,
                ClientSecret = orderCreateRequest.ClientSecret,
            };

            await _unitOfWork.Order.Create(order);
        }

        public async Task UpdateOrderStatus(Guid orderId, OrderStatus status)
        {
            await _unitOfWork.Order.UpdateOrderStatus(orderId, status);
            await _unitOfWork.SaveChangesAsync();

            await _hubContext
                .Clients
                .Group(orderId.ToString())
                .SendAsync("OrderUpdateStatus", new
                {
                    OrderId = orderId,
                    OrderStatus = status,
                    Notification = $"Your Order {orderId} has changed status to {status.ToString()}"
                });
        }

        #endregion

        #region Retrieve 

        public async Task<IEnumerable<OrderDTO>> GetOrders(int userId)
        {
            var orders = await _unitOfWork.Order.GetOrders(userId);
            return orders;
        }

        public async Task<IEnumerable<OrderDTO>> GetOrders()
        {
            var orders = await _unitOfWork.Order.GetOrders();
            return orders;
        }

        #endregion

    }
}
