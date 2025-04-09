using Microsoft.AspNetCore.Authorization;

namespace Store_API.Authorization
{
    public class WarehouseAccessRequirement : IAuthorizationRequirement
    {
        public bool RequireWarehouseAccess { get; }
        public bool RequireSuperAdmin { get; }

        public WarehouseAccessRequirement(bool requireWarehouseAccess = true, bool requireSuperAdmin = false)
        {
            RequireWarehouseAccess = requireWarehouseAccess;
            RequireSuperAdmin = requireSuperAdmin;
        }
    }
} 