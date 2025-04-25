using Microsoft.EntityFrameworkCore;

namespace Store_API.Models.OrderAggregate
{
    [Owned]
    public class ShippingAdress
    {
        public string City { get; set; }
        public string District { get; set; }
        public string Ward { get; set; }
        public string StreetAddress { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
    }
}
