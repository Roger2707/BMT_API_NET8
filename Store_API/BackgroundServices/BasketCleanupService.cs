using Store_API.IService;

namespace Store_API.BackgroundServices
{
    public class BasketCleanupService : BackgroundService
    {
        private readonly ILogger<BasketCleanupService> _logger;
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(10);
        private readonly IServiceScopeFactory _scopeFactory;

        public BasketCleanupService(
            IServiceScopeFactory scopeFactory,
            ILogger<BasketCleanupService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var basketService = scope.ServiceProvider.GetRequiredService<IBasketService>();
                        _logger.LogInformation("Starting basket cleanup service");
                        await basketService.CleanupExpiredBaskets();
                        _logger.LogInformation("Basket cleanup completed successfully");
                    }
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