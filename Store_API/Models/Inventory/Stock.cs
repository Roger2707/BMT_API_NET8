using System.ComponentModel.DataAnnotations.Schema;

namespace Store_API.Models.Inventory
{
    public class Stock
    {
        public Guid Id { get; set; }
        public Guid ProductDetailId { get; set; }
        [ForeignKey("ProductDetailId")]
        public ProductDetail ProductDetail { get; set; }
        public Guid WarehouseId { get; set; }
        [ForeignKey("WarehouseId")]
        public Warehouse Warehouse { get; set; }
        public int Quantity { get; set; }
        public DateTime Updated { get; set; }
    }
}
