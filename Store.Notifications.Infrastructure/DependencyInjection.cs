using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Store.Notifications.Infrastructure.Persistance;
using Store.Notifications.Infrastructure.RabbitMQ.Topology;
using Options = Store.Notifications.Infrastructure.RabbitMQ.Topology.Options;

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

    public static IServiceCollection AddRabbitMq(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<Options>(configuration.GetSection("Rabbit"));

        services.AddSingleton<IConnection>(opt =>
        {
            var options = opt.GetRequiredService<IOptions<Options>>().Value;
            var connection = new ConnectionFactory
            {
                HostName = options.Host,
                UserName = options.Username,
                Password = options.Password,
                VirtualHost = options.VirtualHost
            };

            return connection.CreateConnectionAsync().GetAwaiter().GetResult();
        });

        services.AddSingleton<RabbitMqNotificationsTopology>();
        services.AddSingleton<RetryTopologyDeclaration>();
        services.AddSingleton<DeadTopologyDeclaration>();

        return services;
    }
}