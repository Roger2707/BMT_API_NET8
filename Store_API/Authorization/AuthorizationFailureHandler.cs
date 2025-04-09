using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace Store_API.Authorization
{
    public class AuthorizationFailureHandler : IAuthorizationMiddlewareResultHandler
    {
        private readonly AuthorizationMiddlewareResultHandler _defaultHandler = new();

        public async Task HandleAsync(
            RequestDelegate next,
            HttpContext context,
            AuthorizationPolicy policy,
            PolicyAuthorizationResult authorizeResult)
        {
            // If authorization was successful, continue with the pipeline
            if (authorizeResult.Succeeded)
            {
                await next(context);
                return;
            }

            // Handle authorization failure
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json";

            var errorResponse = new
            {
                StatusCode = StatusCodes.Status403Forbidden,
                Message = "Access denied. You don't have permission to perform this action.",
                Details = new
                {
                    IsAuthenticated = context.User.Identity?.IsAuthenticated ?? false,
                    RequiredPolicy = policy?.Requirements.FirstOrDefault()?.GetType().Name,
                    UserRoles = context.User.Claims
                        .Where(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")
                        .Select(c => c.Value)
                        .ToList(),
                    FailureReasons = authorizeResult.Failure?.FailureReasons.Select(f => f.Message).ToList(),
                    UserId = context.User.FindFirst("sub")?.Value,
                    WarehouseId = context.Request.RouteValues["warehouseId"]?.ToString()
                }
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
        }
    }
} 