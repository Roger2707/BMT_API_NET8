using System.ComponentModel.DataAnnotations.Schema;

namespace Store_API.Models.OrderAggregate
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }

        public int UserAddressId { get; set; }
        [ForeignKey("UserAddressId")]
        public UserAddress UserAddress { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;
        public OrderStatus Status { get; set; }
        public List<OrderItem> Items { get; set; }
        public double DeliveryFee { get; set; }
        public double GrandTotal { get; set; }
    }
}
