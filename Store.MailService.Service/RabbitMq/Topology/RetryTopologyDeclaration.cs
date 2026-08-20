using RabbitMQ.Client;
using Store.Shared.Bus;

namespace Store.MailService.Service.RabbitMq.Topology;

public class RetryTopologyDeclaration(IConfiguration configuration)
{
    public async Task DeclareRetryQueue(
        IChannel channel,
        string mainQueueName,
        string retryQueueName,
        string retryRoutingKey,
        CancellationToken ct)
    {
        var baseTimeToLive = configuration.GetValue<int>("RabbitRetryDelay");

        await DeclareExchanges(channel, ct);

        var xArgs = new Dictionary<string, object?>
        {
            ["x-dead-letter-exchange"] = RabbitMqConstants.Exchange.StoreMailRedelivery,
            ["x-message-ttl"] = baseTimeToLive
        };

        await channel.QueueDeclareAsync(
            queue: retryQueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: xArgs,
            cancellationToken: ct);
        
        await channel.QueueBindAsync(
            queue: retryQueueName,
            exchange: RabbitMqConstants.Exchange.StoreMailRetry,
            routingKey: retryRoutingKey,
            cancellationToken: ct);

        await channel.QueueBindAsync(
            queue: mainQueueName,
            exchange: RabbitMqConstants.Exchange.StoreMailRedelivery,
            routingKey: retryRoutingKey,
            cancellationToken: ct);
    }

    private async Task DeclareExchanges(IChannel channel, CancellationToken ct)
    {
        await channel.ExchangeDeclareAsync(
            RabbitMqConstants.Exchange.StoreMailRetry,
            type: ExchangeType.Topic,
            durable: true,
            cancellationToken: ct);
        
        await channel.ExchangeDeclareAsync(
            RabbitMqConstants.Exchange.StoreMailRedelivery,
            type: ExchangeType.Topic,
            durable: true,
            cancellationToken: ct);
    }
}