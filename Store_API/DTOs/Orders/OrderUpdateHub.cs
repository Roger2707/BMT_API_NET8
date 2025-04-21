using Store_API.Models.OrderAggregate;

namespace Store_API.DTOs.Orders
{
    public class OrderUpdateHub
    {
        public int Id { get; set; }
        public OrderStatus OrderStatus { get; set; }
    }
}
