using System.ComponentModel.DataAnnotations;

namespace Store_API.DTOs.Orders
{
    public class ShippingAddressDTO
    {
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
    }
}
