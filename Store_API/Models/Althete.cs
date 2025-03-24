using System.ComponentModel.DataAnnotations.Schema;

namespace Store_API.Models
{
    public class Althete
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? PictureUrl { get; set; }
        public string? PublicId { get; set; }
        public string Country { get; set; }
        public string Info { get; set; }
        public string Achivement { get; set; }
        public Guid ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product Product { get; set; }
    }
}
