using System.Text.Json;
using Store.Application.Interfaces.RabbitMq;
using Store.Infrastructure.Persistence;
using Store.Infrastructure.RabbitMq.Routing;
using Store.Shared.Bus.EventContracts;


namespace Store.Infrastructure.RabbitMq.Topology;

public sealed class OutboxMessageWriter(StoreContext context) : IOutboxMessageWriter
{
    public async Task SaveMessageAsync(
        IIntegrationEvent message,
        CancellationToken ct)
    {
        var stringPayload = JsonSerializer.Serialize(message, message.GetType());
        var messageRoute = IntegrationRouteResolver.Resolve(message);
        var messageToSave = new OutboxMessage(
            message.EventId,
            message.MessageType,
            messageRoute.Exchange,
            messageRoute.RoutingKey,
            stringPayload);

        await context.OutboxMessages.AddAsync(messageToSave, ct);
    }
}