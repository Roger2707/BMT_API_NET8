using Store_API.Infrastructures;
using Store_API.Services;
using Store_API.Services.IService;

namespace Store_API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            // DB
            services.AddScoped<IDapperService, DapperService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Services
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IBrandService, BrandService>();
            services.AddScoped<IPromotionService, PromotionService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ITechnologyService, TechnologyService>();
            services.AddScoped<IWarehouseService, WarehouseService>();
            services.AddScoped<IStockService, StockService>();
            services.AddScoped<IBasketService, BasketService>();
            services.AddScoped<IInventoryAuthorization, InventoryAuthorizationService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IRatingService, RatingService>();
            services.AddScoped<IShippingOrderService, ShippingOrderService>();
            services.AddScoped<IStockHoldService, StockHoldService>();

            // Others
            services.AddHttpClient();
            services.AddScoped<EmailSenderService>();
            services.AddScoped<IImageService, ImageService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddSingleton<IRedisService, RedisService>();
        }
    }
}
