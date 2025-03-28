using System.ComponentModel.DataAnnotations;

namespace Store_API.DTOs.Products
{
    public class ProductDetailDTO
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public double Price { get; set; }
        public double DiscountPrice { get; set; }
        public string Color { get; set; }
        public string? ExtraName { get; set; } = "";
        public string Status { get; set; }
    }
}
