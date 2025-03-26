namespace Store_API.DTOs.Promotions
{
    public class PromotionDTO
    {
        public Guid Id { get; set; }
        public Guid BrandId { get; set; }
        public string BrandName { get; set; }
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public double PercentageDiscount { get; set; }
    }
}
