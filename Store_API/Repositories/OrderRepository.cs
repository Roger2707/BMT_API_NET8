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
	                            , IIF(o.Status = 0, 'Pending', IIF(o.Status = 1, 'Completed', IIF(o.Status = 2, 'Cancelled', 'Refunded'))) as OrderStatus
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

            var orderGroup = 
                result
                    .GroupBy(o => new 
                            { o.Id, o.UserId, o.FullName, o.Email, o.PhoneNumber, o.OrderDate
                                , o.DeliveryFee, o.OrderStatus, o.GrandTotal
                                , o.City, o.District, o.Ward, o.StreetAddress, o.PostalCode, o.Country 
                            })
                    .Select(g => 
                        new OrderDTO 
                        {
                            Id = g.Key.Id,
                            UserId = g.Key.UserId,
                            FullName = g.Key.FullName,
                            Email = g.Key.Email,
                            PhoneNumber = g.Key.PhoneNumber,
                            OrderDate = g.Key.OrderDate,
                            Status = g.Key.OrderStatus,
                            DeliveryFee = g.Key.DeliveryFee,
                            GrandTotal = g.Key.GrandTotal,

                            UserAddress = new UserAddress
                            {
                                City = g.Key.City,
                                District = g.Key.District,
                                Ward = g.Key.Ward,
                                StreetAddress = g.Key.StreetAddress,
                                PostalCode = g.Key.PostalCode,
                                Country = g.Key.Country,
                            },

                            Items = g.Select(oi => new OrderItemDTO
                            {
                                Id = oi.OrderItemId,
                                ProductId = oi.ProductId,
                                ProductName = oi.ProductName,
                                ProductImageUrl = oi.ProductImageUrl,
                                Quantity = oi.Quantity,
                                SubTotal = oi.SubTotal,
                            }).ToList(),
                        }).FirstOrDefault();

            return orderGroup;
        }
    }
}
