using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Store.Infrastructure.RabbitMq.Publishers.EventContracts;
using Store.Notifications.Infrastructure.RabbitMQ.Topology;
using Store.Shared.Bus;

namespace Store.NotificationsAPI.Features.Notifications.CreateUserEvent;

public class CreateUserEventConsumer(
    IConnection connection,
    ILogger<CreateUserEventConsumer> logger,
    RabbitMqNotificationsTopology topology,
    IServiceScopeFactory serviceScopeFactory
    ) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        await using var channel = await connection.CreateChannelAsync(cancellationToken: ct);
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

                await channel.BasicAckAsync(
                    args.DeliveryTag,
                    multiple: false,
                    cancellationToken: ct);

            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Message processing failed");
                await channel.BasicNackAsync(
                    args.DeliveryTag,
                    multiple: false,
                    requeue: false,
                    cancellationToken: ct);
            }
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