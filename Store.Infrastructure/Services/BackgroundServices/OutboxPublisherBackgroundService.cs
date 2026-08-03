using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Store.Application.Interfaces.RabbitMq;
using Store.Infrastructure.Persistence;
using Store.Infrastructure.RabbitMq.Topology;

namespace Store.Infrastructure.Services.BackgroundServices;

public sealed class OutboxPublisherBackgroundService(
    IServiceScopeFactory scopeFactory,
    IRabbitMqPublisher rabbitMqPublisher,
    IOptions<OutboxBackgroundServiceOptions> outboxOptions) : BackgroundService
{
    private readonly OutboxBackgroundServiceOptions _options = outboxOptions.Value;

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(_options.PublishDelay));

        while (await timer.WaitForNextTickAsync(ct))
        {
            await using var scope = scopeFactory.CreateAsyncScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();

            var messages = await context.OutboxMessages
                .Where(om => om.DeadFromUtc == null
                                && om.ProcessedAtUtc == null
                                && (om.LastFailedAtUtc < DateTime.UtcNow.AddMilliseconds(-_options.BaseRetryInterval * om.Attempts)
                                    || om.LastFailedAtUtc == null))
                .OrderBy(om => om.CreatedAtUtc)
                .Take(_options.BatchSize)
                .ToListAsync(ct);

            var publishMessages = messages
                .Select(om => new OutboxPublishMessage(
                    om.Id,
                    om.Type,
                    om.Exchange,
                    om.RoutingKey,
                    om.Payload)
                )
                .ToList();

            var messagesDictionary = messages.ToDictionary(m => m.Id);
            var publishResults = await rabbitMqPublisher.PublishBatchAsync(publishMessages, ct);

            HandlePublishingResults(publishResults, messagesDictionary);

            await context.SaveChangesAsync(ct);
        }
    }

    private void HandlePublishingResults(List<IPublishResult> publishResults, Dictionary<Guid, OutboxMessage> messagesDictionary)
    {
        foreach (var result in publishResults)
        {
            if (result.IsSuccessful)
            {
                messagesDictionary.TryGetValue(result.Message.Id, out var message);
                message?.MarkAsProcessed();
            }
            else
            {
                messagesDictionary.TryGetValue(result.Message.Id, out var message);
                if (message is null) return;
                message.MarkAsFailed(result.Exception!.Message);
                if (message.Attempts >= _options.MaxAttempts) message.MarkAsDead(result.Exception!.Message);
            }
        }
    }
}