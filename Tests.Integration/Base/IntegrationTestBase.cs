using Microsoft.Extensions.DependencyInjection;
using Store.Domain.Enums;
using Store.Domain.Models;
using Store.Infrastructure.Persistence;

namespace Tests.Integration.Base;

public class IntegrationTestBase(IntegrationTestFixture fixture) : IAsyncLifetime
{
    public async Task InitializeAsync()
    {
        await fixture.ResetDatabaseAsync();
        await using var scope = fixture.Factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<StoreContext>();
        await SeedDataAsync(dbContext);
        await dbContext.SaveChangesAsync();
    }

    protected virtual Task SeedDataAsync(StoreContext context)
    {
        return Task.CompletedTask;
    }

    public Task DisposeAsync() => Task.CompletedTask;
}