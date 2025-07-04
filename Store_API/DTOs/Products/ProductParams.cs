namespace Store_API.DTOs.Products
{
    public class ProductParams
    {
        public double? MinPrice { get; set; } = 0;
        public double? MaxPrice { get; set; } = 100000000;
        public string? SearchBy { get; set; }
        public string? FilterByCategory { get; set; }
        public string? FilterByBrand { get; set; }
        public int CurrentPage { get; set; } = 1;
    }
}
