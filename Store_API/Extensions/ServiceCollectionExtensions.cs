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
            // Service
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IProductService, ProductService>();

            // Others
            services.AddHttpClient();
            services.AddScoped<EmailSenderService>();
            services.AddTransient<IImageRepository, ImageService>();
            services.AddTransient<ICSVRepository, CSVService>();
            services.AddScoped<ITokenRepository, TokenIdentityService>();

            // DB
            services.AddScoped<IDapperService, DapperService>();
            services.AddTransient<IUnitOfWork, UnitOfWork>();
        }
    }
}
