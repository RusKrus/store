namespace Store.Infrastructure.RabbitMq.Topology;

public class RabbitMqOptions
{
    public required string Host { get; init; }
    public required string Username { get; init; }
    public required string Password { get; init; }
    public required string VirtualHost { get; init; }
}