namespace Store_API.IService
{
    public interface IAuthorizationService
    {
        Task<bool> HasWarehouseAccess(int userId, Guid warehouseId);
        Task<bool> HasSpecialAccess(int userId, Guid warehouseId, string permission);
        Task<List<Guid>> GetUserWarehouses(int userId);
    }
} 