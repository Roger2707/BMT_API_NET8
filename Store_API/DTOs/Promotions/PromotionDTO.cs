namespace Store_API.DTOs.Promotions
{
    public class PromotionDTO
    {
        public int Id { get; set; }
        public int BrandId { get; set; }
        public string BrandName { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public double PercentageDiscount { get; set; }
    }
}
