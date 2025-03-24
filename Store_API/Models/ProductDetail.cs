using System.ComponentModel.DataAnnotations.Schema;

namespace Store_API.Models
{
    public class ProductDetail
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product Product { get; set; }
        public double Price { get; set; }
        public int QuantityInStock { get; set; }
        public string Color { get; set; }
        public string? ExtraName { get; set; } = "";
    }
}
