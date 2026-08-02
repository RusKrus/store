namespace Store.Infrastructure.Persistence;

public class OutboxMessage
{
    private OutboxMessage() {}

    public OutboxMessage(
        Guid id,
        string type,
        string exchange,
        string routingKey,
        string payload)
    {
        Id = id;
        Type = type;
        Exchange = exchange;
        RoutingKey = routingKey;
        Payload = payload;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public string Type { get; private set; }
    public string Exchange { get; private set; }
    public string RoutingKey { get; private set; }
    public string Payload { get; private set; } = null!;
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? ProcessedAtUtc { get; private set; }
    public int Attempts { get; private set; }
    public DateTime? LastFailedAtUtc { get; private set; }
    public DateTime? DeadFromUtc { get; private set; }
    public bool IsDead => DeadFromUtc != null;
    public string? LastErrorMessage { get; private set; }

    public void MarkAsProcessed()
    {
        ProcessedAtUtc = DateTime.UtcNow;
        LastErrorMessage = null;
    }

    public void MarkAsFailed(string errorMessage)
    {
        LastErrorMessage = errorMessage;
        LastFailedAtUtc = DateTime.UtcNow;
        Attempts++;

    }

    public void MarkAsDead(string errorMessage)
    {
        LastErrorMessage = errorMessage;
        DeadFromUtc = DateTime.UtcNow;
    }
}