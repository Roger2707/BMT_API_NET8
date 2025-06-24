using Microsoft.AspNetCore.Authorization;
using Store_API.Helpers;
using Store_API.IService;
using Store_API.IRepositories;
using System.Security.Claims;

namespace Store_API.AuthorizationsHandler
{
    public class WarehouseAccessHandler : AuthorizationHandler<WarehouseAccessRequirement>
    {
        private readonly IInventoryAuthorization _authorizationService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUnitOfWork _unitOfWork;

        public WarehouseAccessHandler(
            IInventoryAuthorization authorizationService, 
            IHttpContextAccessor httpContextAccessor,
            IUnitOfWork unitOfWork)
        {
            _authorizationService = authorizationService;
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork = unitOfWork;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, WarehouseAccessRequirement requirement)
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
                    context.Fail();
                }
                return;
            }

            // If warehouse access is required
            if (requirement.RequireWarehouseAccess)
            {
                var warehouseId = _httpContextAccessor.HttpContext.Request.Query["warehouseId"].ToString();
                if (string.IsNullOrEmpty(warehouseId))
                {
                    context.Fail();
                    return;
                }

                var warehouse = await _unitOfWork.Warehouse.GetByIdAsync(Guid.Parse(warehouseId));
                if (warehouse == null)
                {
                    context.Fail();
                    return;
                }

                // Check if warehouse is SuperAdmin-only
                if (warehouse.IsSuperAdminOnly)
                {
                    if (await _authorizationService.IsSuperAdmin(userId))
                    {
                        context.Succeed(requirement);
                    }
                    else
                    {
                        context.Fail();
                    }
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
                    context.Fail();
                    return;
                }
            }

            context.Succeed(requirement);
        }
    }
} 