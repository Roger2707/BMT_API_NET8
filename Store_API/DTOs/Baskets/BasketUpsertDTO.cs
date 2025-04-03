using Store_API.Enums;

namespace Store_API.DTOs.Baskets
{
    public class BasketUpsertDTO
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public Guid ProductDetailId { get; set; }
        public int Quantity { get; set; }
        public BasketMode Mode { get; set; }
    }
}
