using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using Store.Notifications.Infrastructure.RabbitMQ.Topology;
using Store.Shared.Bus;
using Store.Shared.Bus.EventContracts;
using Store.Shared.Bus.Extensions;

namespace Store.NotificationsAPI.Features.Notifications.CreateUserEvent;

public class CreateUserEventConsumer(
    IConnection connection,
    ILogger<CreateUserEventConsumer> logger,
    RabbitMqNotificationsTopology topology,
    IConfiguration configuration,
    IServiceScopeFactory serviceScopeFactory
    ) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        var maxRetryCount = configuration.GetValue<int>("RabbitRetryCount");

        var options = new CreateChannelOptions(
            publisherConfirmationsEnabled: true,
            publisherConfirmationTrackingEnabled: true);
        await using var channel = await connection.CreateChannelAsync(options, cancellationToken: ct);

        await topology.DeclareQueueAsync(channel, ct);
        await channel.BasicQosAsync(0, 1, false, ct);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (_, args) =>
        {
            try
            {
                var message = JsonSerializer.Deserialize<UserRegistered>(args.Body.Span)
                              ?? throw new JsonException();

                // some handler-actions with message
                await using var scope = serviceScopeFactory.CreateAsyncScope();
                var createUserHandler = scope.ServiceProvider.GetRequiredService<CreateUserEventHandler>();
                await createUserHandler.HandleAsync(message, ct);
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "User created notifications message processing failed");
                var retries = args.BasicProperties.GetRetryCount();

                if (retries >= maxRetryCount)
                {
                    await channel.BasicNackAsync(
                        args.DeliveryTag,
                        multiple: false,
                        requeue: false,
                        cancellationToken: ct);
                }
                else
                {
                    var basicProperties = new BasicProperties(args.BasicProperties)
                    {
                        Headers = args.BasicProperties.Headers is null
                            ? new Dictionary<string, object?>()
                            : new Dictionary<string, object?>(args.BasicProperties.Headers)
                    };

                    basicProperties.Headers["RetryCount"] = ++retries;

                    try
                    {
                        await channel.BasicPublishAsync(
                            exchange: RabbitMqConstants.Exchange.StoreNotificationsRetry,
                            routingKey: RabbitMqConstants.RoutingKey.NotificationsUserRegisteredRetry,
                            mandatory: true,
                            basicProperties,
                            body: args.Body,
                            cancellationToken: ct);
                    }
                    catch (PublishException publishException)
                    {
                        logger.LogError(publishException, "Retry message was rejected or returned");
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

                    await channel.BasicAckAsync(
                        args.DeliveryTag,
                        multiple: false,
                        cancellationToken: ct);

                }
                return;
            }

            await channel.BasicAckAsync(
                args.DeliveryTag,
                multiple: false,
                cancellationToken: ct);
        };

        await channel.BasicConsumeAsync(
            queue: RabbitMqConstants.Queue.NotificationsServiceStoreEvents,
            autoAck: false,
            consumer: consumer,
            cancellationToken: ct
        );

        await Task.Delay(Timeout.Infinite, ct);
    }
}