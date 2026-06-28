using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Store.Application.Interfaces;
using Store.Application.Interfaces.CartCookiesService;
using Store.Infrastructure.Auth;
using Store.Infrastructure.Repositories;
using Store.Infrastructure.Extensions;
using Store.Infrastructure.MassTransitEventBus;
using Store.Infrastructure.Services;

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

    public static IServiceCollection AddEventBus(this IServiceCollection services, IConfiguration configuration)
    {
        var password = configuration["Rabbit:Password"];
        var host = configuration["Rabbit:Host"];
        var username = configuration["Rabbit:Username"];

        if (password is null || host is null || username is null)
        {
            throw new Exception("RabbitMQ configuration is missing");
        }

        services.AddMassTransit(mtCfg =>
        {
            mtCfg.UsingRabbitMq((context, rcfg) =>
            {
                rcfg.Host(host,
                    rhconf =>
                    {
                        rhconf.Username(username);
                        rhconf.Password(password);
                    });
            });
        });

        services.AddScoped<IEventBus, EventBus>();
        return services;
    }
}