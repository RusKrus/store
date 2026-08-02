using Store.Shared.Bus.EventContracts;

namespace Store.Infrastructure.RabbitMq.Publishers.EventContracts;

public sealed record UserRegistered(
    Guid EventId,
    int Id,
    string UserName,
    string Email,
    DateTime RegisteredAt) : IIntegrationEvent
{
    public string MessageType => "user_registered_event";
}