using System.Text;
using RabbitMQ.Client;
using Store.Application.Interfaces.RabbitMq;
using Store.Infrastructure.RabbitMq.Topology;
namespace Store.Infrastructure.RabbitMq.Publishers;

public sealed class Publisher(IConnection connection): IRabbitMqPublisher, IAsyncDisposable
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
                var body = Encoding.UTF8.GetBytes(message.Payload);

                var messageTask = SendMessageAsync(message.RoutingKey, message.Exchange, body, _channel, cancellationToken);

                pendingPublishes.Add((message, messageTask));
            }

            var resultList = new List<IPublishResult>(messages.Count);

            foreach (var (message, task) in pendingPublishes)
            {
                try
                {
                    await task;

                    resultList.Add(new OutboxPublishResult(message, true, null));
                }
                catch (Exception e)
                {
                    resultList.Add(new OutboxPublishResult(message, false, e));
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
        string exchange,
        byte[] body,
        IChannel channel,
        CancellationToken cancellationToken)
    {
        var basicProperties = new BasicProperties
        {
            Persistent = true,
            ContentType = "application/json",
            MessageId = Guid.Empty.ToString(),
        };

        return channel.BasicPublishAsync(
            exchange: exchange,
            routingKey: routingKey,
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