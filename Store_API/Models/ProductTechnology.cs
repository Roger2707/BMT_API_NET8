namespace Store_API.Models
{
    public class ProductTechnology
    {
        public Guid ProductId { get; set; }
        public Product Product { get; set; }

        public Guid TechnologyId { get; set; }
        public Technology Technology { get; set; }

        public DateTime Created { get; set; } = DateTime.UtcNow;
    }
}
