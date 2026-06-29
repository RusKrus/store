using System.Text.Json;
using Store.Notifications.Domain.Enums;

namespace Store.Notifications.Domain.Models;

public class Notification
{
    private Notification() {}

    private Notification(
        int userId,
        NotificationTypes type,
        DateTime createdAt,
        string title,
        string message,
        JsonElement payload)
    {
        UserId = userId;
        Type = type;
        CreatedAt = createdAt;
        Title = title;
        Message = message;
        Payload = payload;
    }

    public Guid Id { get; }
    public int UserId { get; }
    public NotificationTypes Type { get; }

    public DateTime CreatedAt { get; }
    public DateTime? ReadAt { get; private set; } = null;
    public bool IsRead => ReadAt != null;

    public string Title { get; }
    public string Message { get; }

    public JsonElement Payload { get; private set; }

    public bool MarkAsRead()
    {
        if (IsRead) return false;
        ReadAt = DateTime.UtcNow;
        return true;
    }

    public static Notification Create<TPayload>(
        int userId,
        NotificationTypes type,
        DateTime createdAt,
        string title,
        string message,
        TPayload payload
    )
    {
        return new Notification(
            userId,
            type,
            createdAt,
            title,
            message,
            JsonSerializer.SerializeToElement(payload)
        );
    }
}