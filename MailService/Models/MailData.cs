namespace MailService.Models;

public class MailData
{
    // Receivers
    public List<string> To { get; }
    public List<string>? Bcc { get; }
    public List<string>? Cc { get; }

    // Sender
    public string? From { get; }
    public string? DisplayName { get; }
    public string? ReplyTo { get; }
    public string? ReplyToName { get; }

    // Content
    public string Subject { get; }
    public string? Body { get; }

    public MailData(
        List<string> to,
        List<string>? bcc,
        List<string>? cc,
        string? from,
        string? displayName,
        string? replyTo,
        string? replyToName,
        string subject,
        string? body)
    {
        // Receiver
        To = to;
        Bcc = bcc ?? [];
        Cc = cc ?? [];
        // Sender
        From = from;
        DisplayName = displayName;
        ReplyTo = replyTo;
        ReplyToName = replyToName;
        // Content
        Subject = subject;
        Body = body;
    }
}