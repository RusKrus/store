using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Store.Database.Postgres.Persistence;

public static class PersistenceExtensions
{
    public static async Task MigrateDbAsync(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<StoreContext>();
        await dbContext.Database.MigrateAsync();
    }
}