﻿using Store_API.DTOs.Orders;
using Store_API.Models.OrderAggregate;

namespace Store_API.Repositories
{
    public interface IOrderRepository
    {
        Task Create(Order order);
        Task<IEnumerable<OrderDTO>> GetOrders(int userId);
        Task<Order> FirstOrDefaultAsync(Guid orderId);
    }
}
