using Microsoft.AspNetCore.Authorization;

namespace Store_API.Authorization
{
    public class WarehouseAccessRequirement : IAuthorizationRequirement
    {
        public string Permission { get; }
        public bool RequireWarehouseAccess { get; }
        public bool RequireSuperAdmin { get; }

        public WarehouseAccessRequirement(string permission = null, bool requireWarehouseAccess = true, bool requireSuperAdmin = false)
        {
            Permission = permission;
            RequireWarehouseAccess = requireWarehouseAccess;
            RequireSuperAdmin = requireSuperAdmin;
        }
    }
} 