using Store.Application.Interfaces.RabbitMq;

namespace Store.Infrastructure.RabbitMq.Topology;

public sealed record PublishResult<T>(T Message, bool IsSuccessful, Exception? Exception) : IPublishResult<T>;