using System.ComponentModel.DataAnnotations.Schema;

namespace Store_API.Models
{
    public class ProductColor
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product Product { get; set; }
        public double Price { get; set; }
        public int QuantityInStock { get; set; }
        public string Color { get; set; }
        public string? ExtraName { get; set; } = "";
    }
}
