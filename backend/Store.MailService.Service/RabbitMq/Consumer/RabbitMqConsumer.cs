using System.Text.Json;
using Dapper;
using Microsoft.Data.Sqlite;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using Store.MailService.Service.Enums;
using Store.MailService.Service.Models;
using Store.MailService.Service.Persistence;
using Store.MailService.Service.Services.Interfaces;
using Store.MailService.Service.RabbitMq.Topology;
using Store.Shared.Bus;
using Store.Shared.Bus.EventContracts;
using Store.Shared.Bus.Extensions;

namespace Store.MailService.Service.RabbitMq.Consumer;

public sealed class RabbitMqConsumer(
  IConnection connection,
  IServiceScopeFactory serviceScopedFactory,
  MailServiceTopology topology,
  SqliteConnectionFactory sqliteConnectionFactory,
  IConfiguration configuration,
  ILogger<RabbitMqConsumer> logger) : BackgroundService
{

  protected override async Task ExecuteAsync(CancellationToken ct)
  {
    var maxRetryCount = configuration.GetValue<int>("RabbitRetryCount");


    var options = new CreateChannelOptions(
        publisherConfirmationsEnabled: true,
        publisherConfirmationTrackingEnabled: true);
    await using var channel = await connection.CreateChannelAsync(options, cancellationToken: ct);
    await channel.BasicQosAsync(0, 1, false, cancellationToken: ct);

    await topology.DeclareAsync(channel, ct);

    var consumer = new AsyncEventingBasicConsumer(channel);
    
    consumer.ReceivedAsync += async (_, args) =>
    {
      try
      {
        var message = JsonSerializer.Deserialize<UserRegistered>(args.Body.Span)
                      ?? throw new JsonException();
        var type = args.BasicProperties.Type;

        await PrepareAndSendEmail(message, type, ct);
      }
      catch (JsonException exception)
      {
        logger.LogError(exception, "Failed to deserialize message");

        await channel.BasicNackAsync(
          args.DeliveryTag,
          multiple: false,
          requeue: false,
          cancellationToken: ct);

        return;
      }
      catch (InvalidOperationException exception)
      {
        await channel.BasicNackAsync(
          args.DeliveryTag,
          multiple: false,
          requeue: false,
          cancellationToken: ct);
      }
      catch (SqliteException exception) when (exception.SqliteExtendedErrorCode == SQLitePCL.raw.SQLITE_CONSTRAINT_PRIMARYKEY)
      {
        logger.LogWarning(exception, "Same mail wanted to be sent several times");
        await channel.BasicAckAsync(
          args.DeliveryTag,
          multiple: false,
          cancellationToken: ct);

        return;
      }
      catch (Exception exception)
      {
        var stringMessageId = args.BasicProperties.MessageId;
        if (stringMessageId is not null && Guid.TryParse(stringMessageId, out var messageId))
        {
          var isAlreadySaved = await CheckIfExists(messageId, ct) > 0;
          if (isAlreadySaved)
          {
            await channel.BasicAckAsync(
              args.DeliveryTag,
              multiple: false,
              cancellationToken: ct);
            return;
          }
        }
        else
        {
          logger.LogError(exception, "Found message without id, sent to DLQ.");
          await channel.BasicNackAsync(
            args.DeliveryTag,
            multiple: false,
            requeue: false,
            cancellationToken: ct);
          return;
        }
      
        
        logger.LogError(exception,  "Failed to process message by mail service");
        var retries = args.BasicProperties.GetRetryCount();

        if (retries >= maxRetryCount)
        {
            await channel.BasicNackAsync(
                args.DeliveryTag,
                multiple: false,
                requeue: false,
                cancellationToken: ct);
            return;
        }

        var basicProperties = new BasicProperties(args.BasicProperties)
        {
            Headers = args.BasicProperties.Headers is null
                ? new Dictionary<string, object?>()
                : new Dictionary<string, object?>(args.BasicProperties.Headers)
        };

        basicProperties.Headers["RetryCount"] = ++retries;

        try
        {
          // retry publish
          await channel.BasicPublishAsync(
            exchange: RabbitMqConstants.Exchange.StoreMailRetry,
            routingKey: RabbitMqConstants.RoutingKey.MailServiceRetry,
            mandatory: true,
            basicProperties,
            body: args.Body,
            cancellationToken: ct);
          
          await channel.BasicAckAsync(
            args.DeliveryTag,
            multiple: false,
            cancellationToken: ct);

          return;
        }
        catch (PublishException publishException) 
        {
          logger.LogError(publishException, "Failed to retry publish message from MailService to retry queue");
          // little backpressure if publisher is not available
          await Task.Delay(TimeSpan.FromMilliseconds(5000), ct);
          // Closed channel will return messages by himself
          if (channel.IsOpen)
          {
              await channel.BasicNackAsync(
                  args.DeliveryTag,
                  multiple: false,
                  requeue: true,
                  cancellationToken: ct);
          }

          return;
        }
        catch (Exception unknownException)
        {
            logger.LogError(unknownException, "Publishing status to retry is unknown");
            // Closed channel will return messages by himself
            if (channel.IsOpen)
            {
                await channel.BasicNackAsync(
                    args.DeliveryTag,
                    multiple: false,
                    requeue: true,
                    cancellationToken: ct);
            }

            return;
        }
      }
      
      await channel.BasicAckAsync(
        args.DeliveryTag,
        multiple: false,
        cancellationToken: ct);
    };

    await channel.BasicConsumeAsync(
      queue: RabbitMqConstants.Queue.MailServiceStoreEvents,
      autoAck: false,
      consumer,
      cancellationToken: ct
    );

    await Task.Delay(Timeout.Infinite, ct);
  }

  private async Task PrepareAndSendEmail(UserRegistered message, string? type, CancellationToken cancellationToken)
  {
    await using var scope = serviceScopedFactory.CreateAsyncScope();

    switch (type)
    {
      case "user_registered_event":
      {
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

          await SaveMessageToDbAsync(
            message.EventId, 
            message.MessageType, 
            mailData, 
            MailStatus.Pending,
            cancellationToken);

          var isSuccess = false;
          try
          {
            isSuccess = await mailService.SendEmailAsync(mailData, cancellationToken);
            if (!isSuccess) logger.LogError("Failed to send email to {email} for event of registration", message.Email);
          }
          catch (Exception sendingMailException)
          {
            logger.LogError(sendingMailException, "Failed to send email to {email} for event of registration", message.Email);
          }

          await ChangeMessageStatusAsync(message.EventId, isSuccess ? MailStatus.Processed : MailStatus.Failed, cancellationToken);
          break;
      }
      default:
      {
          logger.LogError("Unknown message type: {type}", type);
          throw new InvalidOperationException($"Unknown message type: {type}");
      }
    }
  }

  private async Task SaveMessageToDbAsync(
    Guid messageId, 
    string type, 
    MailData message, 
    MailStatus status,
    CancellationToken cancellationToken)
  {
    const string sql = """
                INSERT INTO processed_messages (
                  message_id, 
                  message_type,
                  processed_at, 
                  processing_status,
                  "to",
                  bcc,
                  cc,
                  "from",
                  display_name,
                  reply_to,
                  reply_to_name,
                  subject,
                  body
                )
                VALUES (
                  @MessageId,
                  @MessageType,
                  @ProcessedAt, 
                  @ProcessingStatus,
                  @To,
                  @Bcc,
                  @Cc,
                  @From,
                  @DisplayName,
                  @ReplyTo,
                  @ReplyToName,
                  @Subject,
                  @Body
                );                    
              """;

    var parameters = new
    {
      MessageId = messageId,
      MessageType = type,

      ProcessedAt = DateTime.UtcNow,
      ProcessingStatus = status.ToString(),

      To = JsonSerializer.Serialize(message.To),
      Bcc = JsonSerializer.Serialize(message.Bcc),
      Cc = JsonSerializer.Serialize(message.Cc),
      message.From,
      message.DisplayName,
      message.ReplyTo,
      message.ReplyToName,
      message.Subject,
      message.Body
    };
    
    var command = new CommandDefinition(commandText: sql, parameters: parameters, cancellationToken: cancellationToken);
    await using var dbConnection = sqliteConnectionFactory.CreateConnection();
    await dbConnection.ExecuteAsync(command);
  }

  private async Task ChangeMessageStatusAsync(Guid messageId, MailStatus status, CancellationToken cancellationToken)
  {
    const string sql = "UPDATE processed_messages SET processing_status = @Status WHERE message_id = @MessageId";
    var parameters = new
    {
      Status = status.ToString(),
      MessageId = messageId
    };

    var command = new CommandDefinition(commandText: sql, parameters: parameters, cancellationToken: cancellationToken);
    await using var dbConnection = sqliteConnectionFactory.CreateConnection();
    await dbConnection.ExecuteAsync(command);
  }

  private async Task<int> CheckIfExists(Guid messageId, CancellationToken ct)
  {
    const string sql = "SELECT COUNT(*) FROM processed_messages WHERE message_id = @MessageId";
    var parameters = new { MessageId = messageId };

    var command = new CommandDefinition(sql, parameters, cancellationToken: ct);

    try
    {
      await using var dbConnection = sqliteConnectionFactory.CreateConnection();
      var count = await dbConnection.ExecuteScalarAsync<int>(command);
      return count;
    }
    catch(Exception exception)
    {
      logger.LogError(exception, "Failed to check if message exists in database");
      return 0;
    }

  }
}