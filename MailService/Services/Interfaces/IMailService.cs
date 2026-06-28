using MailService.Models;

namespace MailService.Services.Interfaces;

public interface IMailService
{
    Task <bool> SendEmailAsync(MailData mailData, CancellationToken cancellationToken);
}