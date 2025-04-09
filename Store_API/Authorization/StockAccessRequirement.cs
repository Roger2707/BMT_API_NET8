using Microsoft.AspNetCore.Authorization;

namespace Store_API.Authorization
{
    public class StockAccessRequirement : IAuthorizationRequirement
    {
        public bool RequireSuperAdmin { get; }
        public bool RequireAdminAccess { get; }
        public bool RequireWarehouseAccess { get; }

        public StockAccessRequirement(
            bool requireSuperAdmin = false,
            bool requireAdminAccess = false,
            bool requireWarehouseAccess = true)
        {
            RequireSuperAdmin = requireSuperAdmin;
            RequireAdminAccess = requireAdminAccess;
            RequireWarehouseAccess = requireWarehouseAccess;
        }
    }
} 