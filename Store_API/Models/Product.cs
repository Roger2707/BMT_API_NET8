using System.ComponentModel.DataAnnotations.Schema;

namespace Store_API.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
        public string? ImageUrl { get; set; }
        public string? PublicId { get; set; }
        public int QuantityInStock { get; set; }
        public ProductStatus ProductStatus { get; set; } = ProductStatus.Active;
        public DateTime Created { get; set; } = DateTime.Now;
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Category Category { get; set; }

        public int BrandId { get; set; }
        [ForeignKey("BrandId")]
        public Brand Brand { get; set; }
    }
}
