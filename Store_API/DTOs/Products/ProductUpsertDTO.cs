using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Store_API.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Store_API.DTOs.Products
{
    public class ProductUpsertDTO
    {
        [Required]
        public string Name { get; set; }
        [Required]
        [Range(1, 200)]
        public double Price { get; set; }
        [Required]
        [Length(0, 5000)]
        public string Description { get; set; }
        public IFormFile? ImageUrl { get; set; }
        public string? PublicId { get; set; }
        [Required]
        [Range(1, 100)]
        public int QuantityInStock { get; set; }
        [Required]
        public ProductStatus ProductStatus { get; set; }
        [Required]
        public DateTime Created { get; set; }
        public int CategoryId { get; set; }
        public int BrandId { get; set; }
    }
}
