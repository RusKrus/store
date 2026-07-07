namespace Store.Application.Interfaces;

public interface IRabbitMqPublisher
{
    Task PublishAsync<T>(T message, CancellationToken cancellationToken);
}