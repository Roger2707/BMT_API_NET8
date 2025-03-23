using Store_API.Models;
using System.ComponentModel.DataAnnotations;
namespace Store_API.DTOs.Products
{
    public class ProductUpsertDTO
    {
        [Required]
        [Length(1, 100)]
        public string Name { get; set; }
        [Required]
        [Range(10000, 20000000)]
        public double Price { get; set; }
        [Required]
        [Length(0, 5000)]
        public string Description { get; set; }
        public IFormFileCollection? ImageUrl { get; set; }
        public string? PublicId { get; set; }
        [Required]
        [Range(1, 100)]
        public int QuantityInStock { get; set; }
        [Required]
        public int ProductStatus { get; set; } = 1;
        [Required]
        public DateTime Created { get; set; }
        public int CategoryId { get; set; }
        public int BrandId { get; set; }
    }
}
