namespace Store_API.DTOs.Althetes
{
    public class AltheteDetailDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? PictureUrl { get; set; }
        public string? PublicId { get; set; }
        public string Country { get; set; }
        public string Info { get; set; }
        public string Achivement { get; set; }
        public int? ProductId { get; set; }
        public string? ProductName { get; set; }
    }
}
