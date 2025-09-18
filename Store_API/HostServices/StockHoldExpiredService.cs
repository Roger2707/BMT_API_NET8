using MassTransit;
using Microsoft.EntityFrameworkCore;
using Store_API.Contracts;
using Store_API.Data;
using Store_API.Enums;

namespace Store_API.HostServices
{
    public class StockHoldExpiredService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public StockHoldExpiredService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var db = scope.ServiceProvider.GetRequiredService<StoreContext>();
                        var publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();
                        var now = DateTime.UtcNow;

                        var expiredStockHolds = await db.StockHolds.Where(p => p.Status == StockHoldStatus.Holding && p.ExpiresAt <= now).ToListAsync();
                        if (expiredStockHolds.Any())
                        {
                            foreach (var stockHold in expiredStockHolds)
                            {
                                await publishEndpoint.Publish(new StockHoldExpiredCreated(stockHold.PaymentIntentId));
                            }

                            await db.SaveChangesAsync();
                        }
                    }
                }
                catch (Exception ex)
                {

                }

                // check every 30 seconds
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }
    }
}
