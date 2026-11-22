using Amazon.Runtime;
using Amazon.S3;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Store.Application.Interfaces;
using Store.Application.Interfaces.CartCookiesService;
using Store.Application.Interfaces.RabbitMq;
using Store.Infrastructure.Auth;
using Store.Infrastructure.Repositories;
using Store.Infrastructure.Extensions;
using Store.Infrastructure.ObjectStorage;
using Store.Infrastructure.Persistence;
using Store.Infrastructure.RabbitMq.Outbox;
using Store.Infrastructure.RabbitMq.Publishers;
using Store.Infrastructure.RabbitMq.Topology;
using Store.Infrastructure.Services;
using Store.Infrastructure.Services.BackgroundServices;

namespace Store.Infrastructure;

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
        var connectionString = configuration.GetConnectionString("Rabbit")
                               ?? throw new InvalidOperationException("Rabbit connection string is not defined");

        services.AddOptions<OutboxBackgroundServiceOptions>()
            .BindConfiguration(OutboxBackgroundServiceOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<ExchangeDeclaration>();
        services.AddSingleton<IRabbitMqPublisher, Publisher>();
        services.AddSingleton(new RabbitMqConnectionProvider(connectionString));
        services.AddScoped<IOutboxMessageWriter, OutboxMessageWriter>();

        services.AddHostedService<OutboxPublisherBackgroundService>();
        return services;
    }

    public static IServiceCollection AddObjectStorage(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<ObjectStorageOptions>()
            .BindConfiguration(ObjectStorageOptions.NameSpace)
            .ValidateDataAnnotations()
            .Validate(options =>
                    Uri.TryCreate(options.Endpoint, UriKind.Absolute, out var uri) &&
                    (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps) &&
                    uri.AbsolutePath == "/" &&
                    string.IsNullOrEmpty(uri.Query) &&
                    string.IsNullOrEmpty(uri.Fragment),
                "ObjectStorage: Endpoint is invalid. Must be absolute plain url like http://127.0.0.1:9000"
            )
            .ValidateOnStart();

        services.AddSingleton<IAmazonS3>(serviceProvider =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<ObjectStorageOptions>>().Value;

            return new AmazonS3Client(
                new BasicAWSCredentials(options.AccessKey, options.SecretKey),
                new AmazonS3Config
                {
                    ServiceURL = options.Endpoint,
                    AuthenticationRegion = options.Region,
                    ForcePathStyle = true,
                    RequestChecksumCalculation = RequestChecksumCalculation.WHEN_REQUIRED
                });
        });

        return services;
    }
}