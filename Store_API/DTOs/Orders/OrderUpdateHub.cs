using Store_API.Models.OrderAggregate;

namespace Store_API.DTOs.Orders
{
    public class OrderUpdateHub
    {
        public Guid OrderId { get; set; }
        public OrderStatus OrderStatus { get; set; }
    }
}
