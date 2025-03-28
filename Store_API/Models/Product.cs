using System.ComponentModel.DataAnnotations.Schema;

namespace Store_API.Models
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string PublicId { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public Guid CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Category Category { get; set; }

        public Guid BrandId { get; set; }
        [ForeignKey("BrandId")]
        public Brand Brand { get; set; }

        public List<ProductTechnology> Technologies { get; set; }
        public List<ProductDetail> Details { get; set; }
    }
}
