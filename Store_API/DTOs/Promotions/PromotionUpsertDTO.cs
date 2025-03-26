using Store_API.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Store_API.DTOs.Promotions
{
    public class PromotionUpsertDTO
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public Guid CategoryId { get; set; }
        [Required]
        public Guid BrandId { get; set; }
        public string Start { get; set; }
        public string End { get; set; }
        [Required]
        public double PercentageDiscount { get; set; }
    }
}
