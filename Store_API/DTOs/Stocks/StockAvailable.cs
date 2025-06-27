namespace Store_API.DTOs.Stocks
{
    public class StockAvailable
    {
        public Guid StockId { get; set; }
        public Guid ProductDetailId { get; set; }
        public Guid WarehouseId { get; set; }
        public int Quantity { get; set; }
    }
}
