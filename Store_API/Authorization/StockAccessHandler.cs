using Microsoft.AspNetCore.Authorization;
using Store_API.IService;
using System.Security.Claims;

namespace Store_API.Authorization
{
    public class StockAccessHandler : AuthorizationHandler<StockAccessRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IInventoryAuthorization _inventoryAuthorization;

        public StockAccessHandler(IHttpContextAccessor httpContextAccessor, IInventoryAuthorization inventoryAuthorization)
        {
            _httpContextAccessor = httpContextAccessor;
            _inventoryAuthorization = inventoryAuthorization;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            StockAccessRequirement requirement)
        {
            if (!context.User.Identity?.IsAuthenticated ?? true)
            {
                context.Fail();
                return;
            }

            var userId = int.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var warehouseId = GetWarehouseIdFromRoute();

            // Check for SuperAdmin requirement
            if (requirement.RequireSuperAdmin)
            {
                if (await _inventoryAuthorization.IsSuperAdmin(userId))
                {
                    context.Succeed(requirement);
                    return;
                }
                context.Fail();
                return;
            }

            // Check for Admin access
            if (requirement.RequireAdminAccess)
            {
                if (!await _inventoryAuthorization.IsAdmin(userId) && 
                    !await _inventoryAuthorization.IsSuperAdmin(userId))
                {
                    context.Fail();
                    return;
                }
            }

            // Check warehouse access if required
            if (requirement.RequireWarehouseAccess && warehouseId.HasValue)
            {
                // Super Admin always has access
                if (await _inventoryAuthorization.IsSuperAdmin(userId))
                {
                    context.Succeed(requirement);
                    return;
                }

                var hasAccess = await _inventoryAuthorization.IsWarehouseAdmin(userId, warehouseId.Value);
                if (!hasAccess)
                {
                    context.Fail();
                    return;
                }
            }

            context.Succeed(requirement);
        }

        private Guid? GetWarehouseIdFromRoute()
        {
            var routeData = _httpContextAccessor.HttpContext?.Request.RouteValues;
            if (routeData == null) return null;

            if (routeData.TryGetValue("warehouseId", out var warehouseIdObj) && 
                Guid.TryParse(warehouseIdObj?.ToString(), out var warehouseId))
            {
                return warehouseId;
            }

            return null;
        }
    }
} 