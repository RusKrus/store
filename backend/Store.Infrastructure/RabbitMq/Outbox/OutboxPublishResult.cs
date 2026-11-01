using Store.Application.Interfaces.RabbitMq;

namespace Store.Infrastructure.RabbitMq.Outbox;

public sealed record OutboxPublishResult(
    IOutboxPublishMessage Message,
    bool IsSuccessful,
    Exception? Exception,
    bool IsBrokerError) : IPublishResult;