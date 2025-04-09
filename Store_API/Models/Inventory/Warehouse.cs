namespace Store_API.Models.Inventory
{
    public class Warehouse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public DateTime Created { get; set; }
        public bool IsSuperAdminOnly { get; set; } = false;  // Default to false
    }
}
