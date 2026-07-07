using Store.MailService.Service.Models;

namespace Store.MailService.Service.Services.Interfaces;

public interface IMailService
{
    Task <bool> SendEmailAsync(MailData mailData, CancellationToken cancellationToken);
}