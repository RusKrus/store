using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Store.Application.Interfaces;
using Store.Application.Interfaces.CartCookiesService;
using Store.Infrastructure.Auth;
using Store.Infrastructure.Repositories;
using Store.Infrastructure.Extensions;
using Store.Infrastructure.RabbitMq.Publishers;
using Store.Infrastructure.RabbitMq.Topology;
using Store.Infrastructure.Services;
using Options = Store.Infrastructure.RabbitMq.Topology.Options;

namespace Store.Infrastructure.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPostgres(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<StoreContext>(optionsBuilder =>
        {
            optionsBuilder
                .UseNpgsql(configuration.GetConnectionString("Postgres"))
                .EnableSensitiveDataLogging()
                .LogTo(
                    Console.WriteLine,
                    LogLevel.Information)
                .SeedDb();
        });
        return services;
    }


    public static IServiceCollection AddPostgresRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICartRepository, CartRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        return services;
    }

    public static IServiceCollection AddHasherService(this IServiceCollection services)
    {
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        return services;
    }

    public static IServiceCollection AddJwtService(this IServiceCollection services)
    {
        services.AddScoped<IJwtProvider, JwtProvider>();
        return services;
    }

    public static IServiceCollection AddCartCookiesService(this IServiceCollection services)
    {
        services.AddScoped<ICartCookiesService, CartCookiesService>();
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

        services.AddHostedService<ExchangeDeclaration>();

        services.AddScoped<IRabbitMqPublisher, Publisher>();

        return services;
    }
}