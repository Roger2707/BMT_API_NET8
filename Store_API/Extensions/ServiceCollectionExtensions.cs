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
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IBasketService, BasketService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IPaymentService, PaymentService>();

            // Others
            services.AddHttpClient();
            services.AddScoped<EmailSenderService>();
            services.AddTransient<IImageService, ImageService>();
            services.AddTransient<ICSVRepository, CSVService>();
            services.AddScoped<ITokenRepository, TokenIdentityService>();
            services.AddSingleton<IRabbitMQService, RabbitMQService>();
        }
    }
}
