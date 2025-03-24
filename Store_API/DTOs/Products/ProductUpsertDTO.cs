using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
namespace Store_API.DTOs.Products
{
    public class ProductUpsertDTO
    {
        public Guid? Id { get; set; }
        [Required]
        [Length(1, 100)]
        public string Name { get; set; }
        [Required]
        [Length(0, 5000)]
        public string Description { get; set; }
        public IFormFileCollection? ImageUrl { get; set; }
        public string? PublicId { get; set; }
        [Required]
        public int ProductStatus { get; set; } = 1;
        [Required]
        public DateTime Created { get; set; }
        public int CategoryId { get; set; }
        public int BrandId { get; set; }

        [Required]
        public List<ProductUpsertDetailDTO> ProductDetails { get; set; }
    }
}
