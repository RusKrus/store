using Store.Application.Interfaces.RabbitMq;

namespace Store.Infrastructure.RabbitMq.Topology;

public sealed record OutboxPublishResult(IOutboxPublishMessage Message, bool IsSuccessful, Exception? Exception) : IPublishResult;