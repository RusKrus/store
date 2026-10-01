using Microsoft.Extensions.DependencyInjection;
using Store.Infrastructure.Persistence;

namespace Tests.Integration.Base;

public class IntegrationTestBase(IntegrationTestFixture fixture) : IAsyncLifetime
{
    protected IntegrationTestFixture Fixture => fixture;

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