using Store_API.Models;

namespace Store_API.DTOs.Products
{
    public class ProductDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string? ImageUrl { get; set; }
        public string ProductStatus { get; set; }
        public DateTime Created { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int BrandId { get; set; }
        public string BrandName { get; set; }
        public string BrandCountry { get; set; }
        public List<ProductDetailDTO> Details { get; set; } = new();
    }
}
