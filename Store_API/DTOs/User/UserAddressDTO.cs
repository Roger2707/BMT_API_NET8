namespace Store_API.DTOs.User
{
    public class UserAddressDTO
    {
        public int Id { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string Ward { get; set; }
        public string StreetAddress { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public int UserId { get; set; }
    }
}
