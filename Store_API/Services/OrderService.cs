using Microsoft.AspNetCore.SignalR;
using Store_API.DTOs.Orders;
using Store_API.Enums;
using Store_API.Models.OrderAggregate;
using Store_API.SignalR;
using Store_API.Services.IService;
using Store_API.Infrastructures;

namespace Store_API.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHubContext<NotificationsHub> _hubContext;
        public OrderService(IUnitOfWork unitOfWork, IHubContext<NotificationsHub> hubContext)
        {
            _unitOfWork = unitOfWork;
            _hubContext = hubContext;
        }

        #region Create + Notification

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

        public async Task UpdateOrderStatus(OrderUpdatStatusRequest request)
        {
            await _unitOfWork.Order.UpdateOrderStatus(request);
            await _unitOfWork.SaveChangesAsync();
            int userId = (await _unitOfWork.Order.FirstOrDefaultAsync(request.OrderId)).UserId;
            await _hubContext
                .Clients
                .Group($"user_{userId}")
                .SendAsync("OrderUpdateStatus", new
                {
                    OrderId = request.OrderId.ToString(),
                    OrderStatus = request.OrderStatus
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
