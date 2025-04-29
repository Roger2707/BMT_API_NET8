namespace Store_API.DTOs.Products
{
    public class ProductDetailDisplayDTO
    {
        public Guid ProductId { get; set; }
        public Guid ProductDetailId { get; set; }
        public string ProductName { get; set; }
        public string ImageUrl { get; set; }
        public string Color { get; set; }
        public double Price { get; set; }
        public double DiscountPrice { get; set; }
        public string CategoryName { get; set; }
        public string BrandName { get; set; }
        public string BrandCountry { get; set; }
        public DateTime Created { get; set; }
        public double Stars { get; set; }
        public int TotalRow { get; set; }
    }
}
