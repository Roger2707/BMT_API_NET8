using Store_API.IService;

namespace Store_API.Services
{
    public class BasketBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<BasketBackgroundService> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromSeconds(15);
        public BasketBackgroundService(IServiceProvider serviceProvider, ILogger<BasketBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var basketService = scope.ServiceProvider.GetRequiredService<IBasketService>();

                    // Get all basket keys
                    var keys = await basketService.GetBasketKeysAsync();
                    foreach (var key in keys)
                    {
                        try
                        {
                            var ttl = await basketService.GetBasketTTLAsync(key);
                            if (ttl?.TotalSeconds <= 15)
                            {
                                var username = key.Split(':')[1];
                                await basketService.SyncBasketDB(username);
                                _logger.LogInformation($"Synced basket for user {username} during TTL check");
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"Failed to sync basket for key {key}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during basket sync background service");
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }
        }
    }
}
