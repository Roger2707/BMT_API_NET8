using Microsoft.AspNetCore.Authorization;
using Store_API.Helpers;
using Store_API.IService;
using System.Security.Claims;
using System.Text.Json;

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

            var userId = CF.GetInt(context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var warehouseId = await GetWarehouseIdFromBody();

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

        private async Task<Guid?> GetWarehouseIdFromBody()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null) return null;

            // Only read the body for POST/PUT requests
            if (httpContext.Request.Method != "POST" && httpContext.Request.Method != "PUT")
                return null;

            try
            {
                // Enable buffering if not already enabled
                httpContext.Request.EnableBuffering();
                
                // Read the request body
                using var reader = new StreamReader(httpContext.Request.Body, leaveOpen: true);
                var body = await reader.ReadToEndAsync();
                
                // Reset the position to allow reading again in the controller
                httpContext.Request.Body.Position = 0;

                // Parse the JSON to get the warehouseId
                var jsonDocument = JsonDocument.Parse(body);
                if (jsonDocument.RootElement.TryGetProperty("warehouseId", out var warehouseIdElement))
                {
                    if (Guid.TryParse(warehouseIdElement.GetString(), out var warehouseId))
                    {
                        return warehouseId;
                    }
                }
            }
            catch
            {
                // If parsing fails, ensure we reset the position
                if (httpContext.Request.Body.CanSeek)
                {
                    httpContext.Request.Body.Position = 0;
                }
                return null;
            }

            return null;
        }
    }
} 