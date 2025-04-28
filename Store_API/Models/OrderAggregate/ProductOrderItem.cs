using Microsoft.EntityFrameworkCore;

namespace Store_API.Models.OrderAggregate
{
    [Owned]
    public class ProductOrderItem
    {
        public Guid ProductDetailId { get; set; }
        public string ProductName { get; set; }
        public string ProductImageUrl { get; set; }
        public double ProductPrice { get; set; }
    }
}
