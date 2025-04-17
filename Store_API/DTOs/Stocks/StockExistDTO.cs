namespace Store_API.DTOs.Stocks
{
    public class StockExistDTO
    {
        public Guid WarehouseId { get; set; }
        public string WarehouseName { get; set; }
        public Guid ProductDetailId { get; set; }
        public int Quantity { get; set; }
    }
}
