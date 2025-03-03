using Store_API.Data;
using Store_API.DTOs.Baskets;
using Store_API.DTOs.Orders;
using Store_API.Models;
using Store_API.Models.OrderAggregate;

namespace Store_API.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IDapperService _dapperService;
        private readonly StoreContext _db;
        public OrderRepository(IDapperService dapperService, StoreContext db)
        {
            _dapperService = dapperService;
            _db = db;
        }

        public async Task Create(Order order)
        {
            await _db.Orders.AddAsync(order);
        }

        public async Task<OrderDTO> GetOrder(int orderId)
        {
            string query = @"
                            SELECT 
                                au.Id as UserId
	                            , au.FullName
	                            , au.Email
	                            , au.PhoneNumber
	                            , o.Id
	                            , o.OrderDate
	                            , IIF(o.Status = 0, 'Pending', IIF(o.Status = 1, 'Completed', IIF(o.Status = 2, 'Shipped', 'Cancelled'))) as OrderStatus
	                            , o.DeliveryFee
	                            , o.GrandTotal
                                , o.Id as OrderItemId
	                            , oi.ProductId
	                            , p.Name as ProductName
                                , '' as ProductImageUrl
	                            , oi.Quantity
	                            , oi.SubTotal
	                            , u.City
	                            , u.District
	                            , u.Ward
	                            , u.PostalCode
	                            , u.StreetAddress
	
                            FROM Orders o

                            LEFT JOIN OrderItems oi ON oi.OrderId = o.Id
                            LEFT JOIN Products p ON p.Id = oi.ProductId
                            LEFT JOIN UserAddresses u ON u.UserId = o.UserId 
                            LEFT JOIN AspNetUsers au ON au.Id = u.UserId

                            WHERE o.Id = @Id

                           "
            ;

            List<dynamic> result = await _dapperService.QueryAsync(query, new { Id = orderId });
            if (result.Count == 0) return null;
            var orderItems = new List<OrderItemDTO>();
            foreach (var r in result)
            {
                var orderItem = new OrderItemDTO
                {
                    Id = r.OrderItemId,
                    ProductId = r.ProductId,
                    ProductName = r.ProductName,
                    ProductImageUrl = r.ProductImageUrl,
                    Quantity = r.Quantity,
                    SubTotal = r.SubTotal,
                };
                orderItems.Add(orderItem);
            }

            var order = new OrderDTO
            {
                Id = result[0].Id,
                UserId = result[0].UserId,
                FullName = result[0].FullName,
                Email = result[0].Email,
                PhoneNumber = result[0].PhoneNumber,
                OrderDate = result[0].OrderDate,
                Status = result[0].OrderStatus,

                UserAddress = new UserAddress
                {
                    City = result[0].City,
                    District = result[0].District,
                    Ward = result[0].Ward,
                    StreetAddress = result[0].StreetAddress,
                    PostalCode = result[0].PostalCode,
                    Country = result[0].Country,
                },
                Items = orderItems,
                DeliveryFee = result[0].DeliveryFee,
                GrandTotal = result[0].GrandTotal,
            };

            return order;
        }
    }
}
