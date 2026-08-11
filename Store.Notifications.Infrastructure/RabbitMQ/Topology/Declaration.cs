using RabbitMQ.Client;
using Store.Shared.Bus;

namespace Store.Notifications.Infrastructure.RabbitMQ.Topology;

public class RabbitMqNotificationsTopology(
    RetryTopologyDeclaration retryTopologyDeclaration,
    DeadTopologyDeclaration deadTopologyDeclaration
    )
{
    public async Task DeclareQueueAsync(IChannel channel, CancellationToken ct)
    {
        await deadTopologyDeclaration.DeclareDeadQueue(channel, ct);

        await channel.ExchangeDeclareAsync(
            exchange: RabbitMqConstants.Exchange.StoreEvents,
            type: ExchangeType.Topic,
            durable: true,
            cancellationToken: ct
        );

        var xParams = new Dictionary<string, object?>
        {
            ["x-dead-letter-exchange"] = RabbitMqConstants.Exchange.StoreNotificationsDead,
            ["x-dead-letter-routing-key"] = RabbitMqConstants.RoutingKey.NotificationsDead,
        };

        await channel.QueueDeclareAsync(
            queue: RabbitMqConstants.Queue.NotificationsServiceStoreEvents,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: xParams,
            cancellationToken: ct
        );

        await channel.QueueBindAsync(
            queue: RabbitMqConstants.Queue.NotificationsServiceStoreEvents,
            exchange: RabbitMqConstants.Exchange.StoreEvents,
            routingKey: RabbitMqConstants.RoutingKey.StoreUserCreated,
            cancellationToken: ct
        );

        await retryTopologyDeclaration.DeclareRetryQueue(
            channel,
            RabbitMqConstants.Queue.NotificationsServiceStoreEvents,
            RabbitMqConstants.Queue.NotificationsServiceRetry,
            RabbitMqConstants.RoutingKey.NotificationsServiceRetry,
            ct
        );
    }
}