using Store.Shared.Bus;
using Store.Shared.Bus.EventContracts;

namespace Store.Infrastructure.RabbitMq.Routing;

internal static class IntegrationRouteResolver
{
    public static IntegrationRoute Resolve(IIntegrationEvent message)
    {
        return message switch
        {
            UserRegistered => new IntegrationRoute(
                RabbitMqConstants.Exchange.StoreEvents,
                RabbitMqConstants.RoutingKey.StoreUserCreated),

            _ => throw new ArgumentOutOfRangeException(nameof(message), message, null)
        };
    }
}