using Microsoft.AspNetCore.Authorization;
using Store_API.Authorization;
using Store_API.Models.Users;

namespace Store_API.Extensions
{
    public static class AuthorizationServiceExtensions
    {
        public static void AddAuthorizationServices(this IServiceCollection services)
        {
            // Register HttpContextAccessor for accessing request context
            services.AddHttpContextAccessor();

            // Register the authorization handler
            services.AddScoped<IAuthorizationHandler, WarehouseAccessHandler>();

            // Register the custom authorization failure handler
            services.AddSingleton<IAuthorizationMiddlewareResultHandler, AuthorizationFailureHandler>();

            // Register authorization policies
            services.AddAuthorization(options =>
            {
                // Basic warehouse access - for operations that don't need specific warehouse
                options.AddPolicy("WarehouseAccess", policy =>
                    policy.Requirements.Add(new WarehouseAccessRequirement(requireWarehouseAccess: false)));

                // View specific warehouse details or transactions
                options.AddPolicy("ViewWarehouseDetails", policy =>
                    policy.Requirements.Add(new WarehouseAccessRequirement(requireWarehouseAccess: true)));

                // View transactions in a specific warehouse
                options.AddPolicy("ViewTransactions", policy =>
                    policy.Requirements.Add(new WarehouseAccessRequirement(Permission.VIEW_STOCK, requireWarehouseAccess: true)));

                // Manage transactions in a specific warehouse
                options.AddPolicy("ManageTransactions", policy =>
                    policy.Requirements.Add(new WarehouseAccessRequirement(Permission.MANAGE_STOCK, requireWarehouseAccess: true)));

                // SuperAdmin only operations (create/delete warehouses)
                options.AddPolicy("ManageWarehouses", policy =>
                    policy.Requirements.Add(new WarehouseAccessRequirement(requireSuperAdmin: true, requireWarehouseAccess: false)));
            });
        }
    }
} 