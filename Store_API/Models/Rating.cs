using System.ComponentModel.DataAnnotations;

namespace Store_API.Models
{
    public class Rating
    {
        public int Id { get; set; }
        [Range(0, 5)]
        public double Star { get; set; }
        public Guid ProductId { get; set; }
        public Guid ProductDetailId { get; set; }
        public int UserId { get; set; }

    }
}
