namespace Store.Shared.Bus.EventContracts;

public interface IIntegrationEvent
{
    Guid EventId { get; }
    string MessageType { get; }
}