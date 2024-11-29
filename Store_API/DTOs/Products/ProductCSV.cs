namespace Store_API.DTOs.Products
{
    public class ProductCSV
    {
        public string Name { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public int QuantityInStock { get; set; }
        public int ProductStatus { get; set; }
        public int CategoryId { get; set; }
        public int BrandId { get; set; }
    }
}
