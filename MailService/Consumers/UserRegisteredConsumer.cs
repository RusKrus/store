using MailService.Models;
using MailService.Services.Interfaces;
using MassTransit;
using Store.Shared.Bus.EventContracts;

namespace MailService.Comsumers;

public sealed class UserRegisteredConsumer(
  ILogger<UserRegisteredConsumer> logger,
  IMailService mailService
  ) : IConsumer<UserRegistered>
{
  public async Task Consume(ConsumeContext<UserRegistered> context)
  {
    var message = context.Message;
    logger.LogInformation(
      "Received registration event for {userName} with email {email}",
      message.UserName,
      message.Email
      );

    var mailData = new MailData(
      to: [message.Email],
      bcc: ["belonoir@gmail.com"],
      cc: ["belonoir@gmail.com"],
      from: null,
      displayName: null,
      replyTo: null,
      replyToName: null,
      subject: "Welcome to our store",
      body: $"<h1>Hello, {message.UserName}</h1><p>Your account has been created.</p>"
      );

    var isSuccess = await mailService.SendEmailAsync(mailData, context.CancellationToken);
    if (!isSuccess) logger.LogError("Failed to send email to {email} for event of registration", message.Email);
  }
}