using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Store.MailService.Service.Models;
using Store.MailService.Service.Services.Interfaces;
using Store.Infrastructure.RabbitMq.Publishers.EventContracts;
using Store.MailService.Service.RabbitMq.Topology;
using Store.Shared.Bus;

namespace Store.MailService.Service.RabbitMq.Consumers;

public sealed class RabbitMqConsumer(
  IConnection connection,
  IServiceScopeFactory serviceScopedFactory,
  MailServiceTopology topology,
  ILogger<RabbitMqConsumer> logger) : BackgroundService
{

  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    await using var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);

    await topology.DeclareAsync(channel, stoppingToken);

    await channel.BasicQosAsync(0, 1, false, cancellationToken: stoppingToken);

    var consumer = new AsyncEventingBasicConsumer(channel);
    consumer.ReceivedAsync += async (_, args) =>
    {
      try
      {
        var message = JsonSerializer.Deserialize<UserRegistered>(args.Body.Span)
                      ?? throw new JsonException();

        var result = await PrepareEmailToSend(message, stoppingToken);

        if (!result)
        {
          throw new Exception("Failed to send email");
        }

        await channel.BasicAckAsync(
          args.DeliveryTag,
          multiple: false,
          cancellationToken: stoppingToken
        );
      }
      catch (Exception exception)
      {
        logger.LogError(exception, "Message processing failed");

        await channel.BasicNackAsync(
          args.DeliveryTag,
          multiple: false,
          requeue: false,
          cancellationToken: stoppingToken);
      }
    };

    await channel.BasicConsumeAsync(
      queue: RabbitMqConstants.Queue.MailServiceStoreEvents,
      autoAck: false,
      consumer,
      cancellationToken: stoppingToken
    );

    await Task.Delay(Timeout.Infinite, stoppingToken);
  }

  private async Task<bool> PrepareEmailToSend(UserRegistered message, CancellationToken cancellationToken)
  {
    await using var scope = serviceScopedFactory.CreateAsyncScope();
    var mailService = scope.ServiceProvider.GetRequiredService<IMailService>();

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

    var isSuccess = await mailService.SendEmailAsync(mailData, cancellationToken);
    if (!isSuccess) logger.LogError("Failed to send email to {email} for event of registration", message.Email);

    return isSuccess;
  }
}