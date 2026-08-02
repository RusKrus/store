namespace Store.Infrastructure.RabbitMq.Routing;

public readonly record struct IntegrationRoute(string Exchange, string RoutingKey);