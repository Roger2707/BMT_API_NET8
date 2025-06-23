using Microsoft.EntityFrameworkCore;
using Store_API.Data;
using Store_API.DTOs.Orders;
using Store_API.Enums;
using Store_API.IService;
using Store_API.Models.OrderAggregate;
using Store_API.Models.Users;
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

        #region Crud

        public async Task Create(Order order)
        {
            await _db.Orders.AddAsync(order);
        }

        public async Task UpdateOrderStatus(Guid orderId, OrderStatus status)
        {
            var order  = await _db.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
            if(order == null) throw new Exception("Order not found");

            order.Status = status;
        }

        #endregion

        #region Retrieved

        public async Task<Order> FirstOrDefaultAsync(Guid orderId)
        {
            return await _db.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == orderId);
        }

        public async Task<IEnumerable<OrderDTO>> GetOrders()
        {
            string query = @"
                            SELECT 
                                au.Id as UserId
                                , o.Id

                                , au.FullName
                                , au.Email
                                , au.PhoneNumber

                                , o.OrderDate
                                , IIF(o.Status = 0, 'Paid', IIF(o.Status = 1, 'Cancelled', 'Refunded')) as OrderStatus
                                , o.DeliveryFee
                                , o.GrandTotal
	                            , o.ClientSecret

                                , oi.Id as OrderItemId
                                , oi.ProductDetailId
                                , oi.ProductName as ProductName
                                , oi.ProductImageUrl as ProductImageUrl
                                , oi.Quantity
                                , oi.SubTotal

                                , o.City
                                , o.District
                                , o.Ward
                                , o.PostalCode
                                , o.StreetAddress
                                , o.Country 
	
                            FROM Orders o

                            INNER JOIN OrderItems oi ON oi.OrderId = o.Id
                            INNER JOIN AspNetUsers au ON au.Id = o.UserId
                           "
            ;

            var result = await _dapperService.QueryAsync<OrderDapperRow>(query);
            if (result.Count == 0) return null;
            var orderGroup =
                result
                    .GroupBy(o => new
                    {
                        o.Id,
                        o.UserId,
                        o.FullName,
                        o.Email,
                        o.PhoneNumber,
                        o.OrderDate,
                        o.DeliveryFee,
                        o.OrderStatus,
                        o.GrandTotal,
                        o.ClientSecret,
                        o.City,
                        o.District,
                        o.Ward,
                        o.StreetAddress,
                        o.PostalCode,
                        o.Country
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
                            ClientSecret = g.Key.ClientSecret,
                            GrandTotal = g.Key.GrandTotal,

                            ShippingAddress = new ShippingAddress
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
                                ProductDetailId = oi.ProductDetailId,
                                ProductName = oi.ProductName,
                                ProductImageUrl = oi.ProductImageUrl,
                                Quantity = oi.Quantity,
                                SubTotal = oi.SubTotal,
                            }).ToList(),
                        }).ToList();

            return orderGroup;
        }

        public async Task<IEnumerable<OrderDTO>> GetOrders(int userId)
        {
            string query = @"
                            SELECT 
                                au.Id as UserId
                                , o.Id

                                , au.FullName
                                , au.Email
                                , au.PhoneNumber

                                , o.OrderDate
                                , IIF(o.Status = 0, 'Paid', IIF(o.Status = 1, 'Cancelled', 'Refunded')) as OrderStatus
                                , o.DeliveryFee
                                , o.GrandTotal
	                            , o.ClientSecret

                                , oi.Id as OrderItemId
                                , oi.ProductDetailId
                                , oi.ProductName as ProductName
                                , oi.ProductImageUrl as ProductImageUrl
                                , oi.Quantity
                                , oi.SubTotal

                                , o.City
                                , o.District
                                , o.Ward
                                , o.PostalCode
                                , o.StreetAddress
                                , o.Country 
	
                            FROM Orders o

                            INNER JOIN OrderItems oi ON oi.OrderId = o.Id
                            INNER JOIN AspNetUsers au ON au.Id = o.UserId

                            WHERE au.Id = @Id
                           "
            ;

            var result = await _dapperService.QueryAsync<OrderDapperRow>(query, new { Id = userId });
            if (result.Count == 0) return null;
            var orderGroup =
                result
                    .GroupBy(o => new
                    {
                        o.Id,
                        o.UserId,
                        o.FullName,
                        o.Email,
                        o.PhoneNumber,
                        o.OrderDate,
                        o.DeliveryFee,
                        o.OrderStatus,
                        o.GrandTotal,
                        o.ClientSecret,
                        o.City,
                        o.District,
                        o.Ward,
                        o.StreetAddress,
                        o.PostalCode,
                        o.Country
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
                            ClientSecret = g.Key.ClientSecret,
                            GrandTotal = g.Key.GrandTotal,

                            ShippingAddress = new ShippingAddress
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
                                ProductDetailId = oi.ProductDetailId,
                                ProductName = oi.ProductName,
                                ProductImageUrl = oi.ProductImageUrl,
                                Quantity = oi.Quantity,
                                SubTotal = oi.SubTotal,
                            }).ToList(),
                        }).ToList();

            return orderGroup;
        }

        #endregion
    }
}
