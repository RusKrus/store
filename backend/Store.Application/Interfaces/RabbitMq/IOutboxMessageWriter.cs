using Store.Shared.Bus.EventContracts;

namespace Store.Application.Interfaces.RabbitMq;

public interface IOutboxMessageWriter
{
    Task SaveMessageAsync(
        IIntegrationEvent message,
        CancellationToken ct);
}