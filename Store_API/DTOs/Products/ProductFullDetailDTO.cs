using Store_API.Enums;

namespace Store_API.DTOs.Products
{
    public class ProductFullDetailDTO
    {
        public Guid ProductId { get; set; }
        public Guid ProductDetailId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Color { get; set; }
        public string ExtraName { get; set; }
        public double OriginPrice { get; set; }
        public double PercentageDiscount { get; set; }
        public double DiscountPrice { get; set; }
        public string ImageUrl { get; set; }
        public string PublicId { get; set; }
        public string CategoryName { get; set; }
        public string BrandName { get; set; }
        public string BrandCountry { get; set; }
        public DateTime Created { get; set; }
        public double Stars { get; set; }
        public int TotalRow { get; set; }
        public ProductStatus Status { get; set; }
    }
}
