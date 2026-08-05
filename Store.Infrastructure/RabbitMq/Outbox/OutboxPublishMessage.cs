using Store.Application.Interfaces.RabbitMq;

namespace Store.Infrastructure.RabbitMq.Outbox;

public record OutboxPublishMessage(Guid Id, string Type, string Exchange, string RoutingKey, string Payload) : IOutboxPublishMessage;