using System.ComponentModel.DataAnnotations;

namespace Store_API.DTOs.Products
{
    public class ProductUpsertDetailDTO
    {
        public Guid Id { get; set; }
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
