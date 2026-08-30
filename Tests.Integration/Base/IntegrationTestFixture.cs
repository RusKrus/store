using System.Data.Common;
using Npgsql;
using Respawn;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;

namespace Tests.Integration.Base;

public class IntegrationTestFixture : IAsyncLifetime
{
    private static readonly PostgreSqlContainer PsqContainer = new PostgreSqlBuilder("postgres:latest")
        .WithDatabase("store")
        .WithUsername("postgres")
        .WithPassword("5432")
        .Build();

    private static readonly RabbitMqContainer RabbitMqContainer = new RabbitMqBuilder("rabbitmq:4-management")
        .WithPortBinding("8080", true)
        .WithUsername("store_user")
        .WithPassword("store_password")
        .Build();

    private Respawner _respawner = null!;

    private DbConnection _dbConnection = null!;

    public readonly TestApplicationFactory Factory = new (
        PsqContainer.GetConnectionString(),
        RabbitMqContainer.GetConnectionString()
        );

    public HttpClient Client = null!;

    public async Task InitializeAsync()
    {
        await PsqContainer.StartAsync();
        await RabbitMqContainer.StartAsync();
        _dbConnection = new NpgsqlConnection(PsqContainer.GetConnectionString());
        await _dbConnection.OpenAsync();
        await InitializeRespawnAsync();
        Client = Factory.CreateClient();
    }

    public async Task ResetDatabaseAsync()
    {
        await _respawner.ResetAsync(_dbConnection);
    }

    private async Task InitializeRespawnAsync()
    {
        _respawner = await Respawner.CreateAsync(_dbConnection, new RespawnerOptions
        {
            SchemasToInclude = ["store"],
            DbAdapter = DbAdapter.Postgres
        });
    }

    public async Task DisposeAsync()
    {
        await PsqContainer.DisposeAsync();
        await RabbitMqContainer.DisposeAsync();
        await _dbConnection.DisposeAsync();
        await Factory.DisposeAsync();
        Client.Dispose();
    }
}