namespace Store.Application.Interfaces.RabbitMq;


public interface IRabbitMqPublisher
{
    Task PublishAsync<T>(T message, CancellationToken cancellationToken);

    Task<List<IPublishResult<T>>> PublishBatchAsync<T>(IReadOnlyList<T> messages, CancellationToken cancellationToken);
}