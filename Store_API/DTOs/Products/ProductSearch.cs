namespace Store_API.DTOs.Products
{
    public class ProductSearch
    {
        public string? ProductName { get; set; }
        public double? MinPrice { get; set; }
        public double? MaxPrice { get; set; }
        public Guid? CategoryId { get; set; }
        public Guid? BrandId { get; set; }
    }
}
