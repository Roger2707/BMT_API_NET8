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

            // For SuperAdmin, always succeed for non-SuperAdmin-only operations
            if (await _authorizationService.IsSuperAdmin(userId))
            {
                context.Succeed(requirement);
                return;
            }

            // Requirement checks //

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

            // If warehouse access is required
            if (requirement.RequireWarehouseAccess)
            {
                var warehouseId = _httpContextAccessor.HttpContext.Request.Query["warehouseId"].ToString();
                if (string.IsNullOrEmpty(warehouseId))
                {
                    context.Fail();  // This sets authorizeResult.Succeeded to false
                    return;
                }

                // Check wareHouse access for current user
                // if user is superAdmin -> ok
                // if no -> check admin access
                if (await _authorizationService.IsSuperAdmin(userId))
                {
                    context.Succeed(requirement);
                    return;
                }

                var hasAccess = await _authorizationService.IsWarehouseAdmin(userId, Guid.Parse(warehouseId));
                if (!hasAccess)
                {
                    context.Fail();  // This sets authorizeResult.Succeeded to false
                    return;
                }
            }

            context.Succeed(requirement);
        }
    }
} 