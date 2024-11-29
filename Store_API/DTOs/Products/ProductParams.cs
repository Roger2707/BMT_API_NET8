using System.ComponentModel.DataAnnotations;

namespace Store_API.DTOs.Products
{
    public class ProductParams
    {
        public string? OrderBy { get; set; }
        public string? SearchBy { get; set; }
        public string? FilterByCategory { get; set; }
        public string? FilterByBrand { get; set; }
        public int CurrentPage { get; set; } = 1;
    }
}
