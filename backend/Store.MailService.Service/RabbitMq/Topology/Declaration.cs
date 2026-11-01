using RabbitMQ.Client;
using Store.Shared.Bus;

namespace Store.MailService.Service.RabbitMq.Topology;

public class MailServiceTopology(
        DeadTopologyDeclaration deadTopologyDeclaration,
        RetryTopologyDeclaration retryTopologyDeclaration)
{
    public async Task DeclareAsync(
        IChannel channel, 
        CancellationToken cancellationToken)
    {
        await deadTopologyDeclaration.DeclareDeadQueue(channel, cancellationToken);

        await channel.ExchangeDeclareAsync(
            exchange: RabbitMqConstants.Exchange.StoreEvents,
            type: ExchangeType.Topic,
            durable: true,
            cancellationToken: cancellationToken
        );

        var xParams = new Dictionary<string, object?>
        {
            ["x-dead-letter-exchange"] = RabbitMqConstants.Exchange.StoreMailDead,
            ["x-dead-letter-routing-key"] = RabbitMqConstants.RoutingKey.MailServiceDead,
        };

        await channel.QueueDeclareAsync(
            queue: RabbitMqConstants.Queue.MailServiceStoreEvents,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: xParams,
            cancellationToken: cancellationToken
        );

        await channel.QueueBindAsync(
            queue: RabbitMqConstants.Queue.MailServiceStoreEvents,
            exchange: RabbitMqConstants.Exchange.StoreEvents,
            routingKey: RabbitMqConstants.RoutingKey.StoreUserCreated,
            cancellationToken: cancellationToken
        );

        await retryTopologyDeclaration.DeclareRetryQueue(
            channel,
            RabbitMqConstants.Queue.MailServiceStoreEvents,
            RabbitMqConstants.Queue.MailServiceRetry,
            RabbitMqConstants.RoutingKey.MailServiceRetry,
            cancellationToken);
    }
}