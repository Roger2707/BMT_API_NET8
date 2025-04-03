using Store_API.Models.Users;
using System.ComponentModel.DataAnnotations.Schema;

namespace Store_API.Models
{
    public class Basket
    {
        public Guid Id { get; set; }
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }

        public List<BasketItem> Items { get; set; }
    }

    public class BasketItem
    {
        public Guid Id { get; set; }

        public Guid BasketId { get; set; }
        [ForeignKey("BasketId")]
        public Basket Basket { get; set; }
        public Guid ProductDetailId { get; set; }
        [ForeignKey("ProductDetailId")]
        public ProductDetail ProductDetail { get; set; }
        public int Quantity { get; set; }
        public bool Status { get; set; } = false;
    }
}
