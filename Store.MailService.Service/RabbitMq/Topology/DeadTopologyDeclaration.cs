using RabbitMQ.Client;
using Store.Shared.Bus;

namespace Store.MailService.Service.RabbitMq.Topology;

public class DeadTopologyDeclaration
{
    public async Task DeclareDeadQueue(IChannel channel, CancellationToken ct)
    {
        await channel.ExchangeDeclareAsync(
            exchange: RabbitMqConstants.Exchange.StoreMailDead,
            type: ExchangeType.Topic,
            durable: true,
            cancellationToken: ct);

        await channel.QueueDeclareAsync(
            queue: RabbitMqConstants.Queue.MailServiceDead,
            durable: true, 
            exclusive: false,
            autoDelete: false,
            cancellationToken: ct);

        await channel.QueueBindAsync(
            queue: RabbitMqConstants.Queue.MailServiceDead,
            exchange: RabbitMqConstants.Exchange.StoreMailDead,
            routingKey: RabbitMqConstants.RoutingKey.MailServiceDead,
            cancellationToken: ct);
    }
}