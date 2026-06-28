using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Store.Application.Interfaces;

namespace Store.Infrastructure.Services.BackgroundServices;

public class CartCleaningBackgroundService(
    IServiceScopeFactory scopeFactory,
    IConfiguration configuration,
    ILogger<CartCleaningBackgroundService> logger
    ) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        var interval = configuration.GetValue<int?>("CartCleaning:IntervalMinutes");
        if (interval is null)
        {
            logger.LogWarning("IntervalMinutes is not set");
            return;
        }
        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(interval.Value));
        while (await timer.WaitForNextTickAsync(ct))
        {
            var lifetime = configuration.GetValue<int?>("CartCleaning:CartLifetimeMinutes");
            if (lifetime is null)
            {
                logger.LogWarning("CartLifetimeMinutes is not set");
                continue;
            }

            var olderThan = DateTime.UtcNow.AddMinutes(-lifetime.Value);
            logger.LogInformation("Cleaning carts older than {olderThan}", olderThan);
            using var scope = scopeFactory.CreateScope();
            var cartRepository = scope.ServiceProvider.GetRequiredService<ICartRepository>();
            await cartRepository.CleanOldCartsAsync(olderThan, ct);
        }
    }
}

