namespace Store_API.Models.Users
{
    public static class Permission
    {
        // Warehouse permissions
        public const string MANAGE_WAREHOUSE = "MANAGE_WAREHOUSE";
        public const string VIEW_WAREHOUSE = "VIEW_WAREHOUSE";

        // Stock permissions
        public const string MANAGE_STOCK = "MANAGE_STOCK";
        public const string IMPORT_STOCK = "IMPORT_STOCK";
        public const string EXPORT_STOCK = "EXPORT_STOCK";
        public const string VIEW_STOCK = "VIEW_STOCK";
    }
}
