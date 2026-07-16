using RabbitMQ.Client;
using Store.Shared.Bus;

namespace Store.Notifications.Infrastructure.RabbitMQ.Topology;

public class RabbitMqNotificationsTopology()
{
    public async Task DeclareQueueAsync(IChannel channel, CancellationToken ct)
    {
        await channel.ExchangeDeclareAsync(
            exchange: RabbitMqConstants.Exchange.StoreEvents,
            type: ExchangeType.Topic,
            durable: true,
            cancellationToken: ct
        );

        await channel.QueueDeclareAsync(
            queue: RabbitMqConstants.Queue.NotificationsServiceStoreEvents,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: ct
        );

        await channel.QueueBindAsync(
            queue: RabbitMqConstants.Queue.NotificationsServiceStoreEvents,
            exchange: RabbitMqConstants.Exchange.StoreEvents,
            routingKey: RabbitMqConstants.RoutingKey.StoreUserCreated,
            cancellationToken: ct
            );
    }
}