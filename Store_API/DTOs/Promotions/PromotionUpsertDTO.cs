using System.ComponentModel.DataAnnotations;

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
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        [Required]
        public double PercentageDiscount { get; set; }
    }
}
