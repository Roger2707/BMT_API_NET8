using System.ComponentModel.DataAnnotations.Schema;

namespace Store_API.Models.OrderAggregate
{
    public class UserAddress
    {
        public int Id { get; set; }
        [Column(TypeName = "nvarchar(100)")]
        public string City { get; set; }
        [Column(TypeName = "nvarchar(100)")]
        public string District { get; set; }
        [Column(TypeName = "nvarchar(100)")]
        public string Ward { get; set; }
        [Column(TypeName = "nvarchar(100)")]
        public string StreetAddress { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }

        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}
