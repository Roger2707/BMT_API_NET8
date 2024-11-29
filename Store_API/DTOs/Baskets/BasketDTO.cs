namespace Store_API.DTOs.Baskets
{
    public class BasketDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public List<BasketItemDTO> Items { get; set; }
        public double TotalPrice { get; set; }
        public string PaymentIntentId { get; set; }
        public string ClientSecret { get; set; }
    }
}
