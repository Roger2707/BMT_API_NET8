namespace Store_API.Enums
{
    public enum OrderStatus
    {
        Created = 0,
        Prepared = 1,
        Shipping = 2,
        Shipped = 3,
        Completed = 4,

        Cancelled = 5,
        BackAndRefund = 6,
    }
}
