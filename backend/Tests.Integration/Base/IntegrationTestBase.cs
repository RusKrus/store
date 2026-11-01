using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Store.Infrastructure.Persistence;

namespace Tests.Integration.Base;

public class IntegrationTestBase(IntegrationTestFixture fixture) : IAsyncLifetime
{
    protected IntegrationTestFixture Fixture => fixture;

    protected JsonSerializerOptions JsonReadOptions => fixture.Factory.Services
        .GetRequiredService<IOptions<JsonOptions>>()
        .Value.JsonSerializerOptions;

    /**
     * <summary>
     *  Uses ReadFromJsonAsync with real backend's JSON settings
     * </summary> 
     */
    protected async Task<T?> ReadJsonAsync<T>(HttpResponseMessage response, CancellationToken ct = default)
    {
        return await response.Content.ReadFromJsonAsync<T>(JsonReadOptions, ct);
    }

    protected virtual Task SeedDataAsync(StoreContext context)
    {
        return Task.CompletedTask;
    }

    protected async Task ExecuteWithContext(Action<StoreContext> action)
    {
        await ExecuteWithContext(context =>
        {
            action(context);
            return Task.CompletedTask;
        });
    }

    protected async Task ExecuteWithContext(Func<StoreContext, Task> action)
    {
        await using var scope = fixture.Factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<StoreContext>();
        await action(dbContext);
    }

    protected async Task<TResult> ExecuteWithContext<TResult>(Func<StoreContext, Task<TResult>> action)
    {
        await using var scope = fixture.Factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<StoreContext>();
        return await action(dbContext);
    }

    public async Task InitializeAsync()
    {
        await fixture.ResetDatabaseAsync();
        await using var scope = fixture.Factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<StoreContext>();
        await SeedDataAsync(dbContext);
        await dbContext.SaveChangesAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;
}