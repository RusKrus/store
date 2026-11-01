using System.Text.Json;
using Store.Notifications.Domain.Enums;

namespace Store.Notifications.Domain.Models;

public class Notification
{
    private Notification() {}

    private Notification(
        Guid eventId,
        int userId,
        NotificationTypes type,
        DateTime createdAt,
        string title,
        string message,
        JsonElement payload)
    {
        EventId = eventId;
        UserId = userId;
        Type = type;
        CreatedAt = createdAt;
        Title = title;
        Message = message;
        Payload = payload;
    }

    private Notification(
        Guid eventId,
        int userId,
        NotificationTypes type,
        DateTime createdAt,
        string title,
        string message)
    {
        EventId = eventId;
        UserId = userId;
        Type = type;
        CreatedAt = createdAt;
        Title = title;
        Message = message;
    }

    public Guid Id { get; }
    public Guid EventId { get; private set; }
    public int UserId { get; }
    public NotificationTypes Type { get; }

    public DateTime CreatedAt { get; }
    public DateTime? ReadAt { get; private set; } = null;
    public bool IsRead => ReadAt != null;

    public string Title { get; private set; }
    public string Message { get; private set; }
    public JsonElement? Payload { get; private set; } = null;

    public bool MarkAsRead()
    {
        if (IsRead) return false;
        ReadAt = DateTime.UtcNow;
        return true;
    }

    public static Notification Create<TPayload>(
        Guid eventId,
        int userId,
        NotificationTypes type,
        DateTime createdAt,
        string title,
        string message,
        TPayload payload
    )
    {
        return new Notification(
            eventId,
            userId,
            type,
            createdAt,
            title,
            message,
            JsonSerializer.SerializeToElement(payload)
        );
    }

    public static Notification Create(
        Guid eventId,
        int userId,
        NotificationTypes type,
        DateTime createdAt,
        string title,
        string message
    )
    {
        return new Notification(
            eventId,
            userId,
            type,
            createdAt,
            title,
            message
        );
    }
}