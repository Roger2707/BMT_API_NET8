namespace Store_API.DTOs.Warehouse
{
    public class WarehouseProductQuantity
    {
        public Guid WarehouseId { get; set; }
        public string WarehouseName { get; set; }
        public int Quantity { get; set; }
    }
}
