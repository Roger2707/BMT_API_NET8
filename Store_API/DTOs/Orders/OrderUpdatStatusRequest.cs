using Store_API.Enums;

namespace Store_API.DTOs.Orders
{
    public class OrderUpdatStatusRequest
    {
        public Guid OrderId { get; set; }
        public OrderStatus OrderStatus { get; set; }
    }
}
