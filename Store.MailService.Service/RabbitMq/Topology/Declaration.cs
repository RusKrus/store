using RabbitMQ.Client;
using Store.Shared.Bus;

namespace Store.MailService.Service.RabbitMq.Topology;

public class MailServiceTopology
{
    public async Task DeclareAsync(IChannel channel, CancellationToken cancellationToken)
    {
        await channel.ExchangeDeclareAsync(
            exchange: RabbitMqConstants.Exchange.StoreEvents,
            type: ExchangeType.Topic,
            durable: true,
            cancellationToken: cancellationToken
        );

        await channel.QueueDeclareAsync(
            queue: RabbitMqConstants.Queue.MailServiceStoreEvents,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: cancellationToken
        );

        await channel.QueueBindAsync(
            queue: RabbitMqConstants.Queue.MailServiceStoreEvents,
            exchange: RabbitMqConstants.Exchange.StoreEvents,
            routingKey: RabbitMqConstants.RoutingKey.StoreUserCreated,
            cancellationToken: cancellationToken
        );
    }
}