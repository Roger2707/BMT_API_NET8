namespace Store_API.DTOs.Althetes
{
    public class AltheteUpsertDTO
    {
        public string Name { get; set; }
        public IFormFile PictureUrl { get; set; }
        public string PublicId { get; set; }
        public string Country { get; set; }
        public string Info { get; set; }
        public string Achivement { get; set; }
        public Guid ProductId { get; set; }
    }
}
