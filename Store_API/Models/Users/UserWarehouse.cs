using Store_API.Models.Inventory;
using System.ComponentModel.DataAnnotations.Schema;

namespace Store_API.Models.Users
{
    public class UserWarehouse
    {
        public Guid Id { get; set; }
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
        public Guid WarehouseId { get; set; }
        [ForeignKey("WarehouseId")]
        public Warehouse Warehouse { get; set; }
    }
}
