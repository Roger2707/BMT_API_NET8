using System.ComponentModel.DataAnnotations;

namespace Store_API.DTOs.Stocks
{
    public class StockUpsertDTO
    {
        public Guid StockId { get; set; }
        [Required]
        public Guid ProductDetailId { get; set; }
        public Guid WarehouseId{ get; set; }
        [Required]
        public int TransactionType { get; set; }
        [Required, Range(1, 200)]
        public int Quantity { get; set; }
        public DateTime CurrentDate { get; set; }
    }
}
