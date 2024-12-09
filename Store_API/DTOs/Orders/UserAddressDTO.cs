using System.ComponentModel.DataAnnotations;

namespace Store_API.DTOs.Orders
{
    public class UserAddressDTO
    {
        [Required]
        public string City { get; set; }
        [Required]
        public string District { get; set; }
        [Required]
        public string Ward { get; set; }
        [Required]
        public string StreetAddress { get; set; }
        [Required]
        public string PostalCode { get; set; }
        [Required]
        public string Country { get; set; }
    }
}
