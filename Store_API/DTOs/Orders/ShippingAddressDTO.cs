using Store_API.Models.OrderAggregate;

namespace Store_API.DTOs.Orders
{
    public class ShippingAddressDTO
    {
        public string City { get; set; }
        public string District { get; set; }
        public string Ward { get; set; }
        public string StreetAddress { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public bool IsSaveAddress { get; set; }
    }
}
