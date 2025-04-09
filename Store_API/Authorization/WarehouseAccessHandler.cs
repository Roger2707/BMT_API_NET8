using Microsoft.AspNetCore.Authorization;
using Store_API.Helpers;
using Store_API.IService;
using System.Security.Claims;

namespace Store_API.Authorization
{
    public class WarehouseAccessHandler : AuthorizationHandler<WarehouseAccessRequirement>
    {
        private readonly IInventoryAuthorization _authorizationService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public WarehouseAccessHandler(IInventoryAuthorization authorizationService, IHttpContextAccessor httpContextAccessor)
        {
            _authorizationService = authorizationService;
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,WarehouseAccessRequirement requirement)
        {
            if (!context.User.Identity.IsAuthenticated)
            {
                context.Fail();
                return;
            }

            int userId = CF.GetInt(context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            // For SuperAdmin-only operations (e.g CRUD)
            if (requirement.RequireSuperAdmin)
            {
                if (await _authorizationService.IsSuperAdmin(userId))
                {
                    context.Succeed(requirement);
                }
                else
                {
                    context.Fail();  // This sets authorizeResult.Succeeded to false
                }
                return;
            }

            // For SuperAdmin, always succeed for non-SuperAdmin-only operations
            if (await _authorizationService.IsSuperAdmin(userId))
            {
                context.Succeed(requirement);
                return;
            }

            // If warehouse access is required
            if (requirement.RequireWarehouseAccess)
            {
                var warehouseId = _httpContextAccessor.HttpContext.Request.Query["warehouseId"].ToString();
                if (string.IsNullOrEmpty(warehouseId))
                {
                    context.Fail();  // This sets authorizeResult.Succeeded to false
                    return;
                }

                var hasAccess = await _authorizationService.HasWarehouseAccess(userId, Guid.Parse(warehouseId));
                if (!hasAccess)
                {
                    context.Fail();  // This sets authorizeResult.Succeeded to false
                    return;
                }
            }

            // If specific permission is required
            if (!string.IsNullOrEmpty(requirement.Permission))
            {
                // Only check permission if warehouse access is required
                if (requirement.RequireWarehouseAccess)
                {
                    var warehouseId = _httpContextAccessor.HttpContext.Request.Query["warehouseId"].ToString();
                    var hasPermission = await _authorizationService.HasSpecialAccess(userId, Guid.Parse(warehouseId), requirement.Permission);
                    if (!hasPermission)
                    {
                        context.Fail();  // This sets authorizeResult.Succeeded to false
                        return;
                    }
                }
            }

            context.Succeed(requirement);
        }
    }
} 