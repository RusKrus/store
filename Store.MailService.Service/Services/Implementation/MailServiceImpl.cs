using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using Store.MailService.Service.Configuration;
using Store.MailService.Service.Models;
using Store.MailService.Service.Services.Interfaces;

namespace Store.MailService.Service.Services.Implementation;

public class MailServiceImpl(IOptions<MailSettings> settings, ILogger<MailServiceImpl> logger) : IMailService
{
    private readonly MailSettings _settings = settings.Value;

    public async Task<bool> SendEmailAsync(MailData mailData, CancellationToken cancellationToken)
    {
        try
        {
            var mail = new MimeMessage();

            #region Sender/Receiver
            // sender
            mail.From.Add(new MailboxAddress(_settings.DisplayName, mailData.From ?? _settings.From));
            mail.Sender = new MailboxAddress(mailData.DisplayName ?? _settings.DisplayName, mailData.From ?? _settings.From);

            // receivers
            foreach (var mailaddress in mailData.To)
            {
                mail.To.Add(MailboxAddress.Parse(mailaddress));
            }

            // reply
            if (!string.IsNullOrEmpty(mailData.ReplyTo))
            {
                mail.ReplyTo.Add(new MailboxAddress(mailData.ReplyToName, mailData.ReplyTo));
            }

            if (mailData.Cc is not null)
            {
                foreach (var ccMail in mailData.Cc.Where(address => !string.IsNullOrWhiteSpace(address)))
                {
                    mail.Cc.Add(MailboxAddress.Parse(ccMail));
                }
            }

            if (mailData.Bcc is not null)
            {
                foreach (var bccMail in mailData.Bcc.Where(mail => !string.IsNullOrWhiteSpace(mail)))
                {
                    mail.Bcc.Add(MailboxAddress.Parse(bccMail));
                }
            }
            #endregion

            #region Content

            var body = new BodyBuilder();
            mail.Subject = mailData.Subject;
            body.HtmlBody = mailData.Body;
            mail.Body = body.ToMessageBody();

            #endregion

            #region Send

            using (var client = new SmtpClient())
            {
                try
                {
                    await client.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.StartTls, cancellationToken);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    await client.AuthenticateAsync(_settings.UserName, _settings.Password, cancellationToken);
                    await client.SendAsync(mail, cancellationToken);
                }
                catch(Exception e)
                {
                    logger.LogError(e, e.Message);
                }
                finally
                {
                    await client.DisconnectAsync(true, cancellationToken);
                    client.Dispose();
                }
            }
            #endregion

            return true;
        }
        catch
        {
            return false;
        }

    }
}