using Microsoft.AspNetCore.Authorization;
using Store_API.Authorization;

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
            services.AddScoped<IAuthorizationHandler, StockAccessHandler>();

            // Register the custom authorization failure handler
            services.AddSingleton<IAuthorizationMiddlewareResultHandler, AuthorizationFailureHandler>();

            // Register authorization policies
            services.AddAuthorization(options =>
            {
                #region Warehouse Policies

                options.AddPolicy("WarehouseAccess", policy =>
                    policy.Requirements.Add(new WarehouseAccessRequirement(requireWarehouseAccess: true)));

                options.AddPolicy("ManageWarehouses", policy =>
                    policy.Requirements.Add(new WarehouseAccessRequirement(requireSuperAdmin: true, requireWarehouseAccess: false)));

                #endregion

                #region Stock Policies

                // For viewing stock details and transactions - SuperAdmin and Admin can see
                options.AddPolicy("ViewStockDetails", policy =>
                    policy.Requirements.Add(new StockAccessRequirement(
                        requireSuperAdmin: false,
                        requireAdminAccess: true,
                        requireWarehouseAccess: true)));

                options.AddPolicy("ViewStockTransactions", policy =>
                    policy.Requirements.Add(new StockAccessRequirement(
                        requireSuperAdmin: false,
                        requireAdminAccess: true,
                        requireWarehouseAccess: true)));

                // For managing stock - SuperAdmin can do anything, Admin can only manage their warehouse
                options.AddPolicy("ManageStock", policy =>
                    policy.Requirements.Add(new StockAccessRequirement(
                        requireSuperAdmin: false,
                        requireAdminAccess: true,
                        requireWarehouseAccess: true)));

                #endregion
            });
        }
    }
} 