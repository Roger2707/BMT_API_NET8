namespace Store_API.DTOs.Warehouse
{
    public class WarehouseDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public DateTime Created { get; set; }
    }
}
