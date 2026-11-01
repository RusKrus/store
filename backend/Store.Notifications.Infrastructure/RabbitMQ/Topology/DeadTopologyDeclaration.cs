using RabbitMQ.Client;
using Store.Shared.Bus;

namespace Store.Notifications.Infrastructure.RabbitMQ.Topology;

/*
 * Creates one DLX/DLQ for service
 */
public class DeadTopologyDeclaration
{
    public async Task DeclareDeadQueue(IChannel channel, CancellationToken ct)
    {
        await channel.ExchangeDeclareAsync(
            exchange: RabbitMqConstants.Exchange.StoreNotificationsDead,
            type: ExchangeType.Topic,
            durable: true,
            cancellationToken: ct
        );

        await channel.QueueDeclareAsync(
            queue: RabbitMqConstants.Queue.NotificationsDead,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: ct);

        await channel.QueueBindAsync(
            queue: RabbitMqConstants.Queue.NotificationsDead,
            exchange: RabbitMqConstants.Exchange.StoreNotificationsDead,
            routingKey: RabbitMqConstants.RoutingKey.NotificationsDead,
            cancellationToken: ct);
    }
}