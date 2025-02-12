namespace Store_API.Models.OrderAggregate
{
    public enum OrderStatus
    {
        Pending = 0,
        Completed = 1,
        Shipped = 2,
        Cancelled = 3
    }
}
