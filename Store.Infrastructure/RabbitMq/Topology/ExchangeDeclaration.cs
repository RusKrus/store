using RabbitMQ.Client;
using Store.Shared.Bus;

namespace Store.Infrastructure.RabbitMq.Topology;

public sealed class ExchangeDeclaration
{
    public async Task DeclareExchangesAsync(IChannel channel, CancellationToken cancellationToken)
    {
        await channel.ExchangeDeclareAsync(
            exchange: RabbitMqConstants.Exchange.StoreEvents,
            type: ExchangeType.Topic,
            durable: true,
            cancellationToken: cancellationToken
        );
    }
}