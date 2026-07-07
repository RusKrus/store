using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using Store.Shared.Bus;

namespace Store.Infrastructure.RabbitMq.Topology;

public sealed class ExchangeDeclaration(IConnection connection) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

        await channel.ExchangeDeclareAsync(
            exchange: RabbitMqConstants.Exchange.StoreEvents,
            type: ExchangeType.Topic,
            durable: true,
            cancellationToken: cancellationToken
        );
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}