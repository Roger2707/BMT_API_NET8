namespace Store_API.Models.OrderAggregate
{
    public enum OrderStatus
    {
        Pending = 0,
        Completed = 1,
        Cancelled = 3,
        Refunded = 2
    }
}
