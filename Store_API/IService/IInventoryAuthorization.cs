namespace Store_API.IService
{
    public interface IInventoryAuthorization
    {
        Task<bool> IsSuperAdmin(int userId);
        Task<bool> IsAdmin(int userId);
        Task<bool> IsWarehouseAdmin(int userId, Guid warehouseId);
    }
} 