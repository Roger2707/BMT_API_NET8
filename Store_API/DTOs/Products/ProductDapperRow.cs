namespace Store_API.DTOs.Products
{
    public class ProductDapperRow
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime Created { get; set; }
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public Guid BrandId { get; set; }
        public string BrandName { get; set; }
        public string BrandCountry { get; set; }
        public double Stars {  get; set; }

        // Detail
        public Guid DetailId { get; set; }
        public Guid ProductId { get; set; }
        public double Price { get; set; }
        public double DiscountPrice { get; set; }
        public int Status { get; set; }
        public string Color { get; set; }
        public string? ExtraName { get; set; } = "";
        public int TotalRow { get; set; }
    }
}
