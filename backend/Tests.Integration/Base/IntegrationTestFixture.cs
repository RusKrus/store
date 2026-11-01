using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Respawn;
using Store.Infrastructure.Persistence;
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

    public TestApplicationFactory Factory { get; private set; } = null!;

    public HttpClient Client { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        await PsqContainer.StartAsync();
        await RabbitMqContainer.StartAsync();

        Factory = new TestApplicationFactory(
            PsqContainer.GetConnectionString(),
            RabbitMqContainer.GetConnectionString()
        );
        Client = Factory.CreateClient();

        await using (var scope = Factory.Services.CreateAsyncScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<StoreContext>();
            await dbContext.Database.MigrateAsync();
        }

        _dbConnection = new NpgsqlConnection(PsqContainer.GetConnectionString());
        await _dbConnection.OpenAsync();
        await InitializeRespawnAsync();
    }

    public async Task ResetDatabaseAsync()
    {
        await _respawner.ResetAsync(_dbConnection);
    }

    private async Task InitializeRespawnAsync()
    {
        _respawner = await Respawner.CreateAsync(_dbConnection, new RespawnerOptions
        {
            SchemasToInclude = ["public"],
            DbAdapter = DbAdapter.Postgres
        });
    }

    public async Task DisposeAsync()
    {
        await Factory.DisposeAsync();
        await PsqContainer.DisposeAsync();
        await RabbitMqContainer.DisposeAsync();
        await _dbConnection.DisposeAsync();
        Client.Dispose();
    }
}