using Store_API.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Store_API.DTOs.Promotions
{
    public class PromotionUpsertDTO
    {
        public int CategoryId { get; set; }
        public int BrandId { get; set; }
        public string Start { get; set; }
        public string End { get; set; }
        public double PercentageDiscount { get; set; }
    }
}
