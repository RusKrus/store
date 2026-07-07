using System.Text.Json;
using RabbitMQ.Client;
using Store.Application.Interfaces;
using Store.Infrastructure.RabbitMq.Publishers.EventContracts;
using Store.Shared.Bus;

namespace Store.Infrastructure.RabbitMq.Publishers;

public class Publisher(IConnection connection): IRabbitMqPublisher
{
    private readonly string _exchangeName = RabbitMqConstants.Exchange.StoreEvents;

    public async Task PublishAsync<T>(T message, CancellationToken cancellationToken)
    {
        await using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

        var routingKey = message switch
        {
            UserRegistered => RabbitMqConstants.RoutingKey.StoreUserCreated,
            _ => throw new ArgumentOutOfRangeException(nameof(message), message, null)
        };

        var body = JsonSerializer.SerializeToUtf8Bytes(message);

        var basicProperties = new BasicProperties
        {
            Persistent = true,
            ContentType = "application/json",
        };

        await channel.BasicPublishAsync(
            exchange: _exchangeName,
            routingKey: routingKey,
            mandatory: true,
            basicProperties,
            body,
            cancellationToken);
    }
}