using System.ComponentModel.DataAnnotations.Schema;

namespace Store_API.Models
{
    public class BasketItem
    {
        public int Id { get; set; }

        public int BasketId { get; set; }
        [ForeignKey("BasketId")]
        public Basket Basket { get; set; }
        public Guid ProductDetailId { get; set; }
        [ForeignKey("ProductDetailId")]
        public ProductDetail ProductDetail { get; set; }
        public int Quantity { get; set; }
        public bool Status { get; set; } = false;
    }
}
