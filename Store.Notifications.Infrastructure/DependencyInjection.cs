using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Store.Notifications.Infrastructure.Persistance;

namespace Store.Notifications.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddPostgres(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<NotificationsDbContext>(optionsBuilder =>
        {
            optionsBuilder
                .UseNpgsql(configuration.GetConnectionString("Postgres"))
                .EnableSensitiveDataLogging()
                .LogTo(
                    Console.WriteLine,
                    LogLevel.Information);
        });

        return services;
    }
}