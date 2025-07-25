using System.ComponentModel.DataAnnotations;
namespace Store_API.DTOs.Products
{
    public class ProductUpsertDTO
    {
        [Required]
        [Length(1, 100)]
        public string Name { get; set; }
        [Required]
        [Length(0, 5000)]
        public string Description { get; set; }
        public DateTime Created { get; set; }
        public Guid CategoryId { get; set; }
        public Guid BrandId { get; set; }
        public List<ProductUpsertDetailDTO> ProductDetails { get; set; }
    }
    public class ProductUpsertDetailDTO
    {
        public Guid ProductId { get; set; }
        [Required, Range(100000, 20000000)]
        public double Price { get; set; }
        [Required]
        public string Color { get; set; }
        public string? ExtraName { get; set; }
        [Required, Range(0, 1)]
        public int Status { get; set; }
        public string? ImageUrl { get; set; }
        public string? PublicId { get; set; }
    }
}
