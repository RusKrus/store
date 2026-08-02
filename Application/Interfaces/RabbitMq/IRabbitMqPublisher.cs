using Store.Shared.Bus.EventContracts;

namespace Store.Application.Interfaces.RabbitMq;


public interface IRabbitMqPublisher
{
    Task<List<IPublishResult>> PublishBatchAsync(IReadOnlyList<IOutboxPublishMessage> messages, CancellationToken cancellationToken);
}