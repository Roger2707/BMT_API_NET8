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
        public OrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
                Status = OrderStatus.Paid,
                ShippingAddress = orderCreateRequest.ShippingAdress,
                Items = orderItems,
                GrandTotal = orderCreateRequest.Amount,
                DeliveryFee = orderCreateRequest.Amount > 1000000 ? 0 : 50000,
                ClientSecret = orderCreateRequest.ClientSecret,
            };

            await _unitOfWork.Order.Create(order);
        }

        #endregion

        #region Retrieve 

        public async Task<IEnumerable<OrderDTO>> GetOrders(int userId)
        {
            var orders = await _unitOfWork.Order.GetOrders(userId);
            return orders;
        }

        #endregion

    }
}
