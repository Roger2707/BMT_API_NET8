using Store_API.Models.Users;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Store_API.Models
{
    public class Rating
    {
        public Guid Id { get; set; }
        [Range(0, 5)]
        public double Star { get; set; }
        public Guid ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product Product { get; set; }
        public Guid ProductDetailId { get; set; }
        [ForeignKey("ProductDetailId")]
        public ProductDetail ProductDetail { get; set; }
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}
