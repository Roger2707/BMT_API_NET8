using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Store_API.IService;

namespace Store_API.Services
{
    public class BasketCleanupService : BackgroundService
    {
        private readonly IBasketService _basketService;
        private readonly ILogger<BasketCleanupService> _logger;
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(10);

        public BasketCleanupService(
            IBasketService basketService,
            ILogger<BasketCleanupService> logger)
        {
            _basketService = basketService;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Starting basket cleanup service");
                    await _basketService.CleanupExpiredBaskets();
                    _logger.LogInformation("Basket cleanup completed successfully");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred during basket cleanup");
                }

                try
                {
                    await Task.Delay(_interval, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("Basket cleanup service is stopping");
                    break;
                }
            }
        }
    }
} 