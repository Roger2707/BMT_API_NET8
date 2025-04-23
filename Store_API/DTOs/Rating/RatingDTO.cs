namespace Store_API.DTOs.Rating
{
    public class RatingDTO
    {
        public int UserId { get; set; }
        public Guid ProductId { get; set; }
        public Guid ProductDetailId { get; set; }
        public double Star { get; set; }
    }
}
