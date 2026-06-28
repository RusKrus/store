using Store.Application.Interfaces;
using MassTransit;

namespace Store.Infrastructure.MassTransitEventBus;

public sealed class EventBus(IPublishEndpoint publishEndpoint) : IEventBus
{
    public async Task PublishAsync<TEvent>(
        TEvent @event,
        CancellationToken cancellationToken = default) where TEvent : class
    {
        await publishEndpoint.Publish(@event, cancellationToken);
    }
}