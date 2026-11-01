using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Store.Notifications.Infrastructure.Persistance;

public static class Migrate
{
     public static async Task<IServiceProvider> MigrateAsync(this IServiceProvider services)
     {
          await using var scope = services.CreateAsyncScope();
          var context = scope.ServiceProvider.GetRequiredService<NotificationsDbContext>();
          await context.Database.MigrateAsync();

          return services;
     }
}