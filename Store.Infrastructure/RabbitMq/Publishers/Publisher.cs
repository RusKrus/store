using System.Text.Json;
using RabbitMQ.Client;
using Store.Application.Interfaces;
using Store.Application.Interfaces.RabbitMq;
using Store.Infrastructure.RabbitMq.Publishers.EventContracts;
using Store.Infrastructure.RabbitMq.Topology;
using Store.Shared.Bus;

namespace Store.Infrastructure.RabbitMq.Publishers;

public sealed class Publisher(IConnection connection): IRabbitMqPublisher, IAsyncDisposable
{
    private readonly string _exchangeName = RabbitMqConstants.Exchange.StoreEvents;

    private readonly SemaphoreSlim _channelLock = new(1, 1);
    private readonly SemaphoreSlim _publishLock = new(1, 1);

    private IChannel? _channel;

    public async Task PublishAsync<T>(T message, CancellationToken cancellationToken)
    {
        await _publishLock.WaitAsync(cancellationToken);

        _channel = await GetChannelAsync(cancellationToken);

        var routingKey = GetRoutingKey(message);

        var body = JsonSerializer.SerializeToUtf8Bytes(message);

        SendMessageAsync(routingKey, body, cancellationToken);
    }

    public async Task<List<IPublishResult<T>>> PublishBatchAsync<T>(IReadOnlyList<T> messages, CancellationToken cancellationToken)
    {
        await _publishLock.WaitAsync(cancellationToken);
        try
        {
            _channel = await GetChannelAsync(cancellationToken);

            var pendingPublishes = new List<(T Message, ValueTask Task)>(messages.Count);

            foreach (T message in messages)
            {
                var routingKey = GetRoutingKey(message);
                var body = JsonSerializer.SerializeToUtf8Bytes(message);

                var messageTask = SendMessageAsync(routingKey, body, cancellationToken);

                pendingPublishes.Add((message, messageTask));
            }

            var resultList = new List<IPublishResult<T>>(messages.Count);

            foreach (var (message, task) in pendingPublishes)
            {
                try
                {
                    await task;

                    resultList.Add(new PublishResult<T>(message, true, null));
                }
                catch (Exception e)
                {
                    resultList.Add(new PublishResult<T>(message, false, e));
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
        string routingKey,
        byte[] body,
        CancellationToken cancellationToken)
    {
        var basicProperties = new BasicProperties
        {
            Persistent = true,
            ContentType = "application/json",
            MessageId = Guid.Empty.ToString(),
        };

        return _channel.BasicPublishAsync(
            exchange: _exchangeName,
            routingKey: routingKey,
            mandatory: true,
            basicProperties,
            body,
            cancellationToken);
    }

    private string GetRoutingKey<T>(T message)
    {
        return message switch
        {
            UserRegistered => RabbitMqConstants.RoutingKey.StoreUserCreated,
            _ => throw new ArgumentOutOfRangeException(nameof(message), message, null)
        };
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

            var channel = await connection.CreateChannelAsync(
                options: channelOptions,
                cancellationToken: cancellationToken);

            return channel;
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