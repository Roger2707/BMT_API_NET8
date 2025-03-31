namespace Store_API.DTOs.Products
{
    public class ProductWithDetailDTO
    {
        public Guid ProductDetailId { get; set; }
        public string ImageUrl { get; set; }
        public string Color { get; set; }
        public string CategoryName { get; set; }
        public string BrandName { get; set; }
        public double Price { get; set; }
        public string ProductName { get; set; }
    }
}
