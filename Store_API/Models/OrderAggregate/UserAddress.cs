using System.ComponentModel.DataAnnotations.Schema;

namespace Store_API.Models.OrderAggregate
{
    public class UserAddress
    {
        public int Id { get; set; }
        public string StreetAddress { get; set; }
        public string District { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }

        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}
