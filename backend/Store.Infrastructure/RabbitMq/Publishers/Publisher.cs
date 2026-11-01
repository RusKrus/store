using System.Text;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Store.Application.Interfaces.RabbitMq;
using Store.Infrastructure.Errors.RabbitMq;
using Store.Infrastructure.RabbitMq.Outbox;
using Store.Infrastructure.RabbitMq.Topology;

namespace Store.Infrastructure.RabbitMq.Publishers;

public sealed class Publisher(
    RabbitMqConnectionProvider connectionProvider,
    ExchangeDeclaration exchangeDeclaration,
    ILogger<Publisher> logger
): IRabbitMqPublisher, IAsyncDisposable
{
    private readonly SemaphoreSlim _channelLock = new(1, 1);
    private readonly SemaphoreSlim _publishLock = new(1, 1);

    private IChannel? _channel;

    public async Task<List<IPublishResult>> PublishBatchAsync(
        IReadOnlyList<IOutboxPublishMessage> messages,
        CancellationToken cancellationToken)
    {
        await _publishLock.WaitAsync(cancellationToken);
        try
        {
            _channel = await GetChannelAsync(cancellationToken);

            var pendingPublishes = new List<(IOutboxPublishMessage Message, ValueTask Task)>(messages.Count);

            foreach (var message in messages)
            {
                var messageTask = SendMessageAsync(
                    message,
                    _channel, 
                    cancellationToken);

                pendingPublishes.Add((message, messageTask));
            }

            var resultList = new List<IPublishResult>(messages.Count);

            foreach (var (message, task) in pendingPublishes)
            {
                try
                {
                    await task;

                    resultList.Add(new OutboxPublishResult(message, true, null, false));
                }
                catch (Exception e)
                {
                    resultList.Add(new OutboxPublishResult(message, false, e, false));
                }
            }

            return resultList;
        }
        finally
        {
            _publishLock.Release();
        }
    }

    private ValueTask SendMessageAsync(
        IOutboxPublishMessage message,
        IChannel channel,
        CancellationToken cancellationToken)
    {
        var body = Encoding.UTF8.GetBytes(message.Payload);

        var basicProperties = new BasicProperties
        {
            Persistent = true,
            Type = message.Type,
            ContentType = "application/json",
            MessageId = message.Id.ToString()
        };

        return channel.BasicPublishAsync(
            exchange: message.Exchange,
            routingKey: message.RoutingKey,
            mandatory: true,
            basicProperties,
            body,
            cancellationToken);
    }

    private async Task<IChannel> GetChannelAsync(CancellationToken cancellationToken)
    {
        // opened channel check
        if (_channel is { IsOpen: true })
        {
            return _channel;
        }

        await _channelLock.WaitAsync(cancellationToken);

        try
        {
            // second check for cases when second thread came here before first one finished the job
            if (_channel is { IsOpen: true })
            {
                return _channel;
            }

            // check and dispose for closed existing channels
            if (_channel is not null)
            {
                await _channel.DisposeAsync();
            }

            var channelOptions = new CreateChannelOptions(
                publisherConfirmationsEnabled: true,
                publisherConfirmationTrackingEnabled: true);

            var connection = await connectionProvider.DeclareConnectionAsync(cancellationToken);

            var channel = await connection.CreateChannelAsync(
                options: channelOptions,
                cancellationToken: cancellationToken);

            await exchangeDeclaration.DeclareExchangesAsync(channel, cancellationToken);

            return channel;
        }
        catch (Exception e)
        {
            throw new RabbitMqInfrastructureError($"Error during setting up RabbitMq infrastructure, {e.Message}");
        }
        finally
        {
            _channelLock.Release();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel is not null)
        {
            await _channel.DisposeAsync();
        }

        _publishLock.Dispose();
        _channelLock.Dispose();
    }
}