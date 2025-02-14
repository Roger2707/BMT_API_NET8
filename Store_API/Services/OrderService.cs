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

        public async Task<Order> Create(int userId, UserAddressDTO address, BasketDTO basket)
        {
            // 1. Get Shipping Address
            var orderAddress = new UserAddress
            {
                City = address.City,
                District = address.District,
                Ward = address.Ward,
                StreetAddress = address.StreetAddress,
                PostalCode = address.PostalCode,
                Country = address.Country,
            };

            // 2. Get Items
            var orderItems = new List<OrderItem>();
            foreach (var item in basket.Items)
            {
                if(item.Status == false) continue;

                var orderItem = new OrderItem
                {
                    Quantity = item.Quantity,
                    ProductId = item.ProductId,
                    SubTotal = item.DiscountPrice * item.Quantity,
                };
                orderItems.Add(orderItem);
            }

            // 3. Calc Grand Total
            double grandTotal = 0;
            foreach (var item in orderItems)
            {
                double price = item.SubTotal;
                grandTotal += price;
            }

            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                Status = OrderStatus.Pending,
                UserAddress = orderAddress,
                Items = orderItems,
                GrandTotal = grandTotal,
                DeliveryFee = grandTotal > 100 ? 0 : 10,
                PaymentIntentId = basket.PaymentIntentId,
            };

            // 4. Add Order and Remove/Clear Items in Basket
            await _unitOfWork.Order.Create(order);

            var items = basket.Items.Where(x => x.Status == true).ToList();
            _unitOfWork.Basket.RemoveRange(items);

            // 5. Save 
            await _unitOfWork.SaveChanges();

            return order;
        }

        //public async Task<OrderDTO> GetAll(int userId)
        //{
        //    string query = @"
        //                    SELECT
        //                        o.Id
        //                        , o.UserId
        //                        , u.FullName
        //                        , o.OrderDate
        //                        , 
        //                        CASE
        //                            WHEN o.Status = 0 Then 'Pending'
        //                            WHEN o.Status = 1 Then 'Processing'	
        //                            WHEN o.Status = 2 Then 'Shipped'	
        //                            ELSE 'Cancelled'	
        //                        END as Status
        //                        , o.ShippingAddress_StreetAddress as StreetAddress
        //                        , o.ShippingAddress_City as City
        //                        , o.ShippingAddress_State as State
        //                        , o.ShippingAddress_PostalCode as PostalCode
        //                        , o.ShippingAddress_Country as Country
        //                        , i.Id as OrderItemId
        //                        , i.ProductId
        //                        , p.Name as ProductName
        //                        , p.ImageUrl as ProductImageUrl
        //                        , i.Quantity
        //                        , i.ItemPrice
        //                        , o.TotalPrice
        //                        , o.DeliveryFee

        //                    FROM Orders as o

        //                    INNER JOIN OrderItems i ON i.OrderId = o.Id
        //                    INNER JOIN Products p ON i.ProductId = p.Id
        //                    INNER JOIN AspNetUsers u ON u.Id = o.UserId

        //                    WHERE o.UserId = @UserId

        //                   ";

        //    List<dynamic> result = await _dapperService.QueryAsync(query, new { UserId = userId });
        //    if (result.Count == 0) return null;

        //    var orderItems = new List<OrderItemDTO>();

        //    foreach (var r in result)
        //    {
        //        var orderItem = new OrderItemDTO
        //        {
        //            Id = r.OrderItemId,
        //            ProductId = r.ProductId,
        //            ProductName = r.ProductName,
        //            ProductImageUrl = r.ProductImageUrl,
        //            Quantity = r.Quantity,
        //            ItemPrice = r.ItemPrice,
        //        };
        //        orderItems.Add(orderItem);
        //    }

        //    var order = new OrderDTO
        //    {
        //        Id = result[0].Id,
        //        UserId = result[0].UserId,
        //        FullName = result[0].FullName,
        //        OrderDate = result[0].OrderDate,
        //        Status = result[0].Status,
        //        ShippingAddress = new ShippingAddress
        //        {
        //            StreetAddress = result[0].StreetAddress,
        //            City = result[0].City,
        //            State = result[0].State,
        //            PostalCode = result[0].PostalCode,
        //            Country = result[0].Country,
        //        },
        //        Items = orderItems,
        //        TotalPrice = result[0].TotalPrice,
        //    };

        //    return order;
        //}
    }
}
