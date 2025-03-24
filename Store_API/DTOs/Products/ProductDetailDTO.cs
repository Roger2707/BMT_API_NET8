using System.ComponentModel.DataAnnotations;

namespace Store_API.DTOs.Products
{
    public class ProductDetailDTO
    {
        public Guid Id { get; set; }
        [Required]
        public Guid ProductId { get; set; }
        [Required]
        [Range(100000, 20000000)]
        public double Price { get; set; }
        [Required]
        [Range(100000, 20000000)]
        public double DiscountPrice { get; set; }
        [Required]
        [Range(1, 100)]
        public int QuantityInStock { get; set; }
        [Required]
        public string Color { get; set; }
        public string? ExtraName { get; set; } = "";
    }
}
