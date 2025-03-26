namespace Store_API.Models
{
    public class Technology
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string? ImageUrl { get; set; }
        public string? PublicId { get; set; }
        public List<ProductTechnology>? Products { get; set; }
    }
}
