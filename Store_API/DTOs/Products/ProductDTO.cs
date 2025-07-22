namespace Store_API.DTOs.Products
{
    public class ProductDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Created { get; set; }
        public string CategoryName { get; set; }
        public string BrandName { get; set; }
        public string BrandCountry { get; set; }
        public int TotalRow { get; set; }
        public List<ProductDetailDTO> Details { get; set; } = new();
    }

    public class ProductDetailDTO
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public double OriginPrice { get; set; }
        public double PercentageDiscount { get; set; }
        public double DiscountPrice { get; set; }
        public string? ImageUrl { get; set; }
        public string? PublicId { get; set; }
        public string Color { get; set; }
        public string? ExtraName { get; set; } = "";
        public string Status { get; set; }
        public double Stars { get; set; }

    }
}
