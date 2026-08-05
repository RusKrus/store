using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Store.Infrastructure.RabbitMq.Topology;

public class RabbitMqConnectionProvider(IOptions<RabbitMqOptions> rabbitOptions) : IAsyncDisposable
{
    private readonly SemaphoreSlim _connectionDeclarationLock = new (1, 1);
    private readonly RabbitMqOptions _options = rabbitOptions.Value;
    private IConnection? _connection;

    public async Task<IConnection> DeclareConnectionAsync(CancellationToken ct) 
    {
        // opened connection check
        if (_connection is not null) return _connection;

        await _connectionDeclarationLock.WaitAsync(ct);
        
        try
        {
            // second check for parallel thread
            if (_connection is not null)
            {
                return _connection;
            }

            var factory = new ConnectionFactory
            {
                HostName = _options.Host,
                UserName = _options.Username,
                Password = _options.Password,
                VirtualHost = _options.VirtualHost,
                ClientProvidedName = "app:store",
                AutomaticRecoveryEnabled = true,
                TopologyRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(5)
            };

            _connection = await factory.CreateConnectionAsync(ct);
            return _connection;
        }
        finally
        {
            _connectionDeclarationLock.Release();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_connection is not null)
        {
            await _connection.DisposeAsync();
        }

        _connectionDeclarationLock.Dispose();
    }
}