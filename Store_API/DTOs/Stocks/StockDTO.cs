namespace Store_API.DTOs.Stocks
{
    public class StockDTO
    {
        public Guid ProductDetailId { get; set; }
        public string ProductName { get; set; }
        public string Color { get; set; }
        public double Price { get; set; }
        public string ImageUrl { get; set; }
        public string CategoryName { get; set; }
        public string BrandName { get; set; }

        public List<StockDetailDTO>? StockDetail { get; set; }
    }

    public class StockDetailDTO
    {
        public Guid StockId { get; set; }
        public Guid WarehouseId { get; set; }
        public string WarehouseName { get; set; }
        public int Quantity { get; set; }
    }

    public class StockDapperRow
    {
        public Guid StockId { get; set; }

        public Guid ProductDetailId { get; set; }
        public string ProductName { get; set; }
        public string Color { get; set; }
        public double Price { get; set; }
        public string ImageUrl { get; set; }
        public string CategoryName { get; set; }
        public string BrandName { get; set; }
        public Guid WarehouseId { get; set; }
        public string WarehouseName { get; set; }

        public int Quantity { get; set; }
    }

    public class StockWarehouseDTO
    {
        public Guid StockId { get; set; }

        public Guid ProductDetailId { get; set; }
        public string ProductName { get; set; }
        public string Color { get; set; }
        public double Price { get; set; }
        public Guid WarehouseId { get; set; }
        public string WarehouseName { get; set; }

        public int Quantity { get; set; }
        public DateTime Updated { get; set; }
    }
}
