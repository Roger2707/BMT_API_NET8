namespace Store_API.DTOs.Baskets
{
    public class BasketItemDTO
    {
        public int BasketItemId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public double OriginPrice { get; set; }
        public double SalesPrice { get; set; }
        public double Discount { get; set; }
        public bool Status { get; set; }
    }
}
