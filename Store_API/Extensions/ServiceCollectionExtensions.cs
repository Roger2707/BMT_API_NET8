using Store_API.Cache_Layer;
using Store_API.Data;
using Store_API.IService;
using Store_API.Repositories;
using Store_API.Services;

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


            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IPaymentService, PaymentService>();

            // Others
            services.AddHttpClient();
            services.AddScoped<EmailSenderService>();
            services.AddScoped<IImageService, ImageService>();
            services.AddScoped<ICSVService, CSVService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddSingleton<IRabbitMQService, RabbitMQService>();
            services.AddSingleton<IRedisService, RedisService>();
        }
    }
}
