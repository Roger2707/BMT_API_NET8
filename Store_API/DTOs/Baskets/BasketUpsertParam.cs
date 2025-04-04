using Store_API.Enums;

namespace Store_API.DTOs.Baskets
{
    public class BasketUpsertParam
    {
        public Guid ProductDetailId { get; set; }
        public int Quantity { get; set; }
        public BasketMode Mode { get; set; }
    }
}
