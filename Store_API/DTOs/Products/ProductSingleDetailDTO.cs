namespace Store_API.DTOs.Products
{
    public class ProductSingleDetailDTO
    {
        public Guid ProductDetailId { get; set; }
        public string ProductName { get; set; }
        public string ProductFirstImage { get; set; }
        public double OriginPrice { get; set; }
        public double DiscountPercent { get; set; }
        public double DiscountPrice { get; set; }
        public string BrandName { get; set; }
        public string CategoryName { get; set; }
    }
} 