using Microsoft.EntityFrameworkCore;
using Store_API.Data;
using Store_API.DTOs.Orders;
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

        public async Task Create(Order order)
        {
            await _db.Orders.AddAsync(order);
        }

        public async Task<Order> FirstOrDefaultAsync(Guid orderId)
        {
            return await _db.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == orderId);
        }

        public async Task<OrderDTO> GetOrder(Guid orderId)
        {
            string query = @"
                            SELECT 
                                au.Id as UserId
                                , o.Id

                                , au.FullName
                                , au.Email
                                , au.PhoneNumber

                                , o.OrderDate
                                , IIF(o.Status = 0, 'Pending', IIF(o.Status = 1, 'Shipping', IIF(o.Status = 2, 'Completed', IIF(o.Status = 3, 'Cancelled', 'Refunded')))) as OrderStatus
                                , o.DeliveryFee
                                , o.GrandTotal
	                            , o.ClientSecret

                                , oi.Id as OrderItemId
                                , oi.ProductDetailId
                                , p.Name as ProductName
                                , p.ImageUrl as ProductImageUrl
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
                            INNER JOIN ProductDetails pd ON pd.Id = oi.ProductDetailId
                            INNER JOIN Products p ON p.Id = pd.ProductId
                            INNER JOIN AspNetUsers au ON au.Id = u.UserId

                            WHERE o.Id = @Id
                           "
            ;

            var result = await _dapperService.QueryAsync<OrderDapperRow>(query, new { Id = orderId });
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

                            ShippingAdress = new ShippingAdress
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
                        }).FirstOrDefault();

            return orderGroup;
        }

        public async Task<IEnumerable<OrderItemDapperRow>> GetOrder(string clientSecret)
        {
            string query = @"
                            SELECT 
	                            o.Id
                                , o.OrderDate
                                , IIF(o.Status = 0, 'Pending', IIF(o.Status = 1, 'Shipping', IIF(o.Status = 2, 'Completed', IIF(o.Status = 3, 'Cancelled', 'Refunded')))) as OrderStatus
                                , o.DeliveryFee
                                , o.GrandTotal
	                            , o.ClientSecret

                                , oi.Id as OrderItemId
                                , oi.ProductDetailId
                                , p.Name as ProductName
                                , p.ImageUrl as ProductImageUrl
                                , oi.Quantity
                                , oi.SubTotal

	
                            FROM Orders o

                            LEFT JOIN OrderItems oi ON oi.OrderId = o.Id
                            LEFT JOIN ProductDetails pd ON pd.Id = oi.ProductDetailId
                            LEFT JOIN Products p ON p.Id = pd.ProductId

                            WHERE o.ClientSecret = @ClientSecret
                           "
            ;

            var result = await _dapperService.QueryAsync<OrderItemDapperRow>(query, new { ClientSecret = clientSecret });
            if (result.Count == 0) return null;
            return result;
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
                                , IIF(o.Status = 0, 'Pending', IIF(o.Status = 1, 'Shipping', IIF(o.Status = 2, 'Completed', IIF(o.Status = 3, 'Cancelled', 'Refunded')))) as OrderStatus
                                , o.DeliveryFee
                                , o.GrandTotal
	                            , o.ClientSecret

                                , oi.Id as OrderItemId
                                , oi.ProductDetailId
                                , p.Name as ProductName
                                , p.ImageUrl as ProductImageUrl
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
                            INNER JOIN ProductDetails pd ON pd.Id = oi.ProductDetailId
                            INNER JOIN Products p ON p.Id = pd.ProductId
                            INNER JOIN AspNetUsers au ON au.Id = u.UserId

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

                            ShippingAdress = new ShippingAdress
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
    }
}
