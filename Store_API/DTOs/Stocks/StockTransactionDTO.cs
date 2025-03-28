namespace Store_API.DTOs.Stocks
{
    public class StockTransactionDTO
    {
        public Guid Id { get; set; }

        public Guid ProductDetailId { get; set; }
        public string ProductName { get; set; }
        public string Color { get; set; }
        public double Price { get; set; }

        public Guid WarehouseId { get; set; }
        public string WarehouseName { get; set; }

        public int Quantity { get; set; }
        public string TransactionType { get; set; }
        public DateTime Created { get; set; }
    }
}
