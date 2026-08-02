namespace Store.Application.Interfaces.RabbitMq;

public interface IOutboxPublishMessage
{
    Guid Id { get; }
    string Type { get; }
    string Exchange { get; }
    string RoutingKey { get; }
    string Payload { get; }
}