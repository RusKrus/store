using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using Store.Shared.Bus;

namespace Store.Notifications.Infrastructure.RabbitMQ.Topology;

public class RetryTopologyDeclaration(IConfiguration configuration)
{
    /**
     * Creates retry queue with for specified main queue.
     * Retry queue redeliveries message with retry key to the same main queue
     */
    public async Task DeclareRetryQueue(
        IChannel channel,
        string mainQueueName,
        string retryQueueName,
        string retryRoutingKey,
        CancellationToken ct)
    {
        var baseTimeToLive = configuration.GetValue<int>("RabbitRetryDelay");

        await DeclareRetryExchanges(channel, ct);

        var xArgs = new Dictionary<string, object?>
        {
            ["x-dead-letter-exchange"] = RabbitMqConstants.Exchange.StoreNotificationsRedelivery,
            ["x-message-ttl"] = baseTimeToLive
        };

        await channel.QueueDeclareAsync(
            queue: retryQueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: xArgs,
            cancellationToken: ct
        );

        await channel.QueueBindAsync(
            queue: retryQueueName,
            exchange: RabbitMqConstants.Exchange.StoreNotificationsRetry,
            routingKey: retryRoutingKey,
            cancellationToken: ct
        );

        // binding main queue to redelivery exchange
        await channel.QueueBindAsync(
            queue: mainQueueName,
            exchange: RabbitMqConstants.Exchange.StoreNotificationsRedelivery,
            routingKey: retryRoutingKey,
            cancellationToken: ct
        );
    }

    /*
     * Creates 2 essential exchanges per service - retry from consumers, and redelivery to consumers
     */
    private async Task DeclareRetryExchanges(IChannel channel, CancellationToken ct)
    {
        await channel.ExchangeDeclareAsync(
            exchange: RabbitMqConstants.Exchange.StoreNotificationsRetry,
            type: ExchangeType.Topic,
            durable: true,
            cancellationToken: ct
        );

        await channel.ExchangeDeclareAsync(
            exchange: RabbitMqConstants.Exchange.StoreNotificationsRedelivery,
            type: ExchangeType.Topic,
            durable: true,
            cancellationToken: ct
        );
    }
}