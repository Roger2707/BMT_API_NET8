using Store_API.Models.OrderAggregate;
using System.ComponentModel.DataAnnotations.Schema;

namespace Store_API.Models.Users
{
    public class UserAddress
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
        public ShippingAdress ShippingAddress { get; set; }
    }
}
