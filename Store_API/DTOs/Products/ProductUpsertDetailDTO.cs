using System.ComponentModel.DataAnnotations;

namespace Store_API.DTOs.Products
{
    public class ProductUpsertDetailDTO
    {
        public Guid ProductId { get; set; }
        [Required, Range(100000, 20000000)]
        public double Price { get; set; }
        [Required, Range(1, 100)]
        public int QuantityInStock { get; set; }
        public string Color { get; set; }
        public string ExtraName { get; set; }
    }
}
