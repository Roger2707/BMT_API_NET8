namespace Store_API.Models.OrderAggregate
{
    public enum OrderStatus
    {
        Pending = 0,
        Shipping = 1,
        Completed = 2,
        Cancelled = 3,
        Refunded = 4
    }
}
