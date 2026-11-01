namespace Store.MailService.Service.Configuration;

public class MailSettings
{
    public string? DisplayName { get; init; }
    public string? From { get; init; }
    public string? UserName { get; init; }
    public string? Password { get; init; }
    public string? Host { get; init; }
    public int Port { get; init; }
    public bool UseSsl { get; init; }
    public bool UseStartTls { get; init; }
    public bool UseOauth { get; init; }

}