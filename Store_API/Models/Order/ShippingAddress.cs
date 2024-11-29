using Microsoft.EntityFrameworkCore;

namespace Store_API.Models.Order
{
    [Owned]
    public class ShippingAddress
    {
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
    }
}
